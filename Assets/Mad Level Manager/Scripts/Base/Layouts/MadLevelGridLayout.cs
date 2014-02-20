/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using MadLevelManager;
using System;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

[ExecuteInEditMode]
public class MadLevelGridLayout : MadLevelAbstractLayout {

    // ===========================================================
    // Constants
    // ===========================================================
    
    private static readonly Color ColorOpaque = new Color(1, 1, 1, 1);
    private static readonly Color ColorTransparent = new Color(1, 1, 1, 0);
    
    // ===========================================================
    // Fields
    // ===========================================================
    
    public SetupMethod setupMethod = SetupMethod.Generate;
    
    public MadSprite rightSlideSprite;
    public MadSprite leftSlideSprite;
    
    public Vector2 iconScale = Vector2.one;
    public Vector2 iconOffset;
    
    public Vector2 rightSlideScale = Vector2.one;
    public Vector2 rightSlideOffset;
    
    public Vector2 leftSlideScale = Vector2.one;
    public Vector2 leftSlideOffset;
    
    public int gridWidth = 3;
    public int gridHeight = 3;
    
    public int pixelsWidth = 720;
    public int pixelsHeight = 578;
    
    public bool pagesOffsetFromResolution = true;
    public float pagesOffsetManual = 1000;
    
    public bool lookAtLastLevel = true;

    [HideInInspector] [NonSerialized] public bool dirty;
    [HideInInspector] [NonSerialized] public bool deepClean;
    private int hash; // for dirtness check
    
    MadDragStopDraggable draggable;
    MadSprite slideLeft, slideRight;
    
    List<Page> pages = new List<Page>();
    int pageCurrentIndex = 0;
    
    // hides managed objects for user to not look into it
    public bool hideManagedObjects = true;
    
    
    // deprecated    
    [SerializeField]
    private Vector2 _iconScale = Vector2.one;
    
    // ===========================================================
    // Properties
    // ===========================================================
    
    MadSprite[] slideIcons {
        get { return new MadSprite[] { slideLeft, slideRight }; }
    }
    
    float pagesXOffset {
        get {
            if (pagesOffsetFromResolution) {
                var root = MadTransform.FindParent<MadRootNode>(transform);
                var loc = root.ScreenGlobal(1, 0);
                
                loc = transform.InverseTransformPoint(loc);
                
                return loc.x * 2;
            } else {
                return pagesOffsetManual;
            }
        }
    }
    
    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================
    
    public override MadLevelIcon GetIcon(string levelName) {
        var icon = MadTransform.FindChild<MadLevelIcon>(transform, (i) => i.level.name == levelName);
        if (icon != null) {
            return icon;
        }
        
        return null;
    }
    
    public override MadLevelIcon FindClosestIcon(Vector3 position) {
        MadLevelIcon closestIcon = null;
        float closestDistance = float.MaxValue;
        
        var icons = MadTransform.FindChildren<MadLevelIcon>(transform);
        foreach (var icon in icons) {
            float distance = Vector3.Distance(position, icon.transform.position);
            if (distance < closestDistance) {
                closestIcon = icon;
                closestDistance = distance;
            }
        }
        
        return closestIcon;
    }
    
    public override void LookAtIcon(MadLevelIcon icon) {
        int pageIndex = PageIndexForLevel(icon.level.name);
        SwitchPage(pageIndex);
    }
    
    // ===========================================================
    // Methods
    // ===========================================================
    
    void OnValidate() {
        gridWidth = Mathf.Max(1, gridWidth);
        gridHeight = Mathf.Max(1, gridHeight);
        pixelsWidth = Mathf.Max(1, pixelsWidth);
        pixelsHeight = Mathf.Max(1, pixelsHeight);
    }
    
    protected override void OnEnable() {
        base.OnEnable();
        Upgrade();
    }
    
    void Upgrade() {
        // upgrading icon scale when it was set to other value than default
        if (_iconScale != Vector2.one) {
            iconScale = _iconScale;
            _iconScale = Vector2.one;
        }
    }
    
    protected override void Start() {
        base.Start();
        
        UpdateLayout(false);
        if (lookAtLastLevel) {
            LookAtLastPlayedLevel();
        }
    }
    
    protected override void Update() {
        base.Update();
        
        UpdateLayout(false);
        SlideIconsUpdate();
    }
    
    void UpdateLayout(bool forceDelete) {
        if (IsDirty()) {
            CleanUp(forceDelete || deepClean);
            Build();
            MakeClean();
            
            configuration.callbackChanged = () => {
                if (this != null) {
                    CleanUp(forceDelete);
                    Build();
                    MakeClean();
                }
            };
        }
    }
    
#region SlideIcons
    void SlideIconsUpdate() {
        if (slideLeft == null || slideRight == null) {
            return;
        }
    
        slideLeft.tint = HasPrevPage() ? ColorOpaque : ColorTransparent;
        slideRight.tint = HasNextPage() ? ColorOpaque : ColorTransparent;
        
        if (draggable.dragging) {
            SlideIconsHide();
        }
    }

    void SlideIconsHide() {
        foreach (var si in slideIcons) {
            si.tint = ColorTransparent;
        }
    }
#endregion

#region Pages
    bool HasNextPage() {
        return pageCurrentIndex + 1 < pages.Count;
    }
    
    bool HasPrevPage() {
        return pageCurrentIndex > 0;
    }
    
    void GoToNextPage() {
        SwitchPage(pageCurrentIndex + 1);
    }
    
    void GoToPrevPage() {
        SwitchPage(pageCurrentIndex - 1);
    }
    
    void SwitchPage(int newIndex) {
        MadDebug.Assert(newIndex >= 0 && newIndex < pages.Count, "There's no page with index " + newIndex);
        pageCurrentIndex = newIndex;
        draggable.MoveTo(newIndex);
    }
    
    int PageIndexForLevel(string levelName) {
        var configuration = MadLevelConfiguration.GetActive();
        int index = configuration.FindLevelIndex(MadLevel.Type.Level, levelName);
        int levelsPerPage = gridWidth * gridHeight;
        int pageIndex = index / levelsPerPage;
        return pageIndex;
    }
#endregion
    
    bool IsDirty() {
        int currentHash = ComputeHash();
    
        if (dirty) {
            hash = currentHash;
            return true;
        }
    
        if (configuration == null || iconTemplate == null) {
            hash = currentHash;
            return false;
        }
        
        if (hash != currentHash) {
            hash = currentHash;
            return true;
        }
        
        return false;
    }
    
    int ComputeHash() {
        var hashCode = new MadHashCode();
        hashCode.Add(configuration);
        hashCode.Add(hideManagedObjects);
        hashCode.Add(setupMethod);
        hashCode.Add(iconTemplate);
        hashCode.Add(iconScale);
        hashCode.Add(iconOffset);
        hashCode.Add(leftSlideSprite);
        hashCode.Add(leftSlideScale);
        hashCode.Add(leftSlideOffset);
        hashCode.Add(rightSlideSprite);
        hashCode.Add(rightSlideScale);
        hashCode.Add(rightSlideOffset);
        hashCode.Add(gridWidth);
        hashCode.Add(gridHeight);
        hashCode.Add(pixelsWidth);
        hashCode.Add(pixelsHeight);
        hashCode.Add(pagesOffsetManual);
        hashCode.Add(pagesOffsetFromResolution);
        
        return hashCode.GetHashCode();
    }
    
    void MakeClean() {
        dirty = false;
        deepClean = false;
    }
    
    void CleanUp(bool forceDelete) {
        int levelCount = configuration.LevelCount(MadLevel.Type.Level);
        var children = MadTransform.FindChildren<MadLevelIcon>(transform, (icon) => icon.hasLevelConfiguration);
        
        if (forceDelete) {
            foreach (var child in children) {
                MadGameObject.SafeDestroy(child.gameObject);
            }
        } else {
            var sorted = from c in children orderby c.levelIndex ascending select c;
        
            foreach (MadLevelIcon levelIcon in sorted) {
                if (levelIcon.levelIndex >= levelCount) {
                    // disable leftovers
                    MadGameObject.SetActive(levelIcon.gameObject, false);
                }
            }
        }
    }
    
    // builds level icons that are absent now
    void Build() {
        // create or get a draggable
        draggable = MadTransform.GetOrCreateChild<MadDragStopDraggable>(transform, "Draggable");
        
        draggable.dragStopCallback = (index) => {
            pageCurrentIndex = index;
        };
    
        float startX = -pixelsWidth / 2;
        float startY = pixelsHeight / 2;
        
        float dx = pixelsWidth / (gridWidth + 1);
        float dy = -pixelsHeight / (gridHeight + 1);
        
        int levelNumber = 1;
        
        MadLevelIcon previousIcon = null;
        
        int levelCount = configuration.LevelCount(MadLevel.Type.Level);
        int levelIndex = 0;
        
        int pageIndex = 0;
        
        while (levelIndex < levelCount) {
            Transform page = MadTransform.FindChild<Transform>(draggable.transform, (t) => t.name == "Page " + (pageIndex + 1));
            bool createPageInstance = page == null;
                        
            if (createPageInstance) {
                page = MadTransform.CreateChild<Transform>(draggable.transform, "Page " + (pageIndex + 1));
            }
            
            if (createPageInstance || generate) {
                page.localPosition = new Vector3(pagesXOffset * pageIndex, 0, 0);
            }
            
        
            for (int y = 1; y <= gridHeight && levelIndex < levelCount; ++y) {
                for (int x = 1; x <= gridWidth && levelIndex < levelCount; ++x, levelIndex++) {
                
                    // update info: in previous versions page was nested directly under draggable
                    // now they should be placed inside "Page X" transforms
                    MadLevelIcon levelIcon;

                    // look directly under Draggable                    
                    levelIcon = MadTransform.FindChild<MadLevelIcon>(
                        draggable.transform, (ic) => ic.levelIndex == levelIndex, 0);
                    if (levelIcon != null) {
                        // move to page
                        levelIcon.transform.parent = page;
                    } else {
                        // find under the page
                        levelIcon = MadTransform.FindChild<MadLevelIcon>(
                            page.transform, (ic) => ic.levelIndex == levelIndex, 0);
                    }
                        
                    bool createNewInstance = levelIcon == null;
                    var level = configuration.GetLevel(MadLevel.Type.Level, levelIndex);
                    
                    if (createNewInstance) {
                        levelIcon = MadTransform.CreateChild(
                            page.transform, level.name, iconTemplate);
                    } else {
                        levelIcon.name = level.name;
                    }
                    
                    levelIcon.gameObject.hideFlags = generate ? HideFlags.HideInHierarchy : 0;
                    
                    if (!MadGameObject.IsActive(levelIcon.gameObject)) {
                        MadGameObject.SetActive(levelIcon.gameObject, true);
                    }
                    
                    levelIcon.levelIndex = levelIndex;
                    levelIcon.configuration = configuration;
                    levelIcon.hasLevelConfiguration = true;
                    
                    if (generate || createNewInstance) {
                        levelIcon.pivotPoint = MadSprite.PivotPoint.Center;
                        
                        levelIcon.transform.localPosition =
                            new Vector3(startX + dx * x + iconOffset.x, startY + dy * y + iconOffset.y, 0);
                            
                        levelIcon.transform.localScale = new Vector3(iconScale.x, iconScale.y, 1);
                    
                        if (levelIcon.levelNumber != null) {
                            levelIcon.levelNumber.text = levelNumber.ToString();
                        }
                    }
                    
                    if (previousIcon != null) {
                        if (createNewInstance) {
                            previousIcon.unlockOnComplete.Add(levelIcon);
                        }
                    } else {
                        if (generate) {
                            levelIcon.locked = false;
                        }
                    }
                    
                    previousIcon = levelIcon;
                    levelNumber++;
                }
            }
            
            pageIndex++;
        }
    
        BuildSlideIcons();    
        BuildDragging(draggable, (int) Mathf.Ceil((float) levelCount / (gridWidth * gridHeight)));
        
        // enable/disable selection based on hide settings
        var sprites = GetComponentsInChildren<MadSprite>();
        foreach (var sprite in sprites) {
            sprite.editorSelectable = !generate;
        }
    }
    
    void BuildDragging(MadDragStopDraggable dragHandler, int dragStops) {
        var pages = MadTransform.FindChildren<Transform>(dragHandler.transform, (t) => t.name.StartsWith("Page"), 0);
        pages.Sort((a, b) => { return a.localPosition.x.CompareTo(b.localPosition.x); });
    
        for (int i = 0; i < pages.Count; ++i) {
            int dragStopIndex = dragHandler.AddDragStop(pages[i].localPosition.x, 0);
            var page = new Page(dragStopIndex);
            this.pages.Add(page);
        }
    }
    
    void BuildSlideIcons() {
        if (leftSlideSprite == null || rightSlideSprite == null) {
            Debug.LogWarning("Slide prefabs not set yet.");
            return;
        }
    
        slideLeft = BuildSlide(leftSlideSprite, "SlideLeftAnchor", true);
        slideRight = BuildSlide(rightSlideSprite, "SlideRightAnchor", false);
        
        slideLeft.transform.localScale = new Vector3(leftSlideScale.x, leftSlideScale.y, 1);
        slideRight.transform.localScale = new Vector3(rightSlideScale.x, rightSlideScale.y, 1);
        
        slideLeft.transform.localPosition += (Vector3) leftSlideOffset;
        slideRight.transform.localPosition += (Vector3) rightSlideOffset;
        
        MadSprite.Action goToPrevPage = (sprite) => {
            if (HasPrevPage()) {
                GoToPrevPage();
            }
        };
        slideLeft.onTap += goToPrevPage;
        slideLeft.onMouseUp += goToPrevPage;
        
        MadSprite.Action goToNextPage = (sprite) => {
            if (HasNextPage()) {
                GoToNextPage();
            }
        };
        slideRight.onTap += goToNextPage;
        slideRight.onMouseUp += goToNextPage;
    }
    
    MadSprite BuildSlide(MadSprite template, string anchorName, bool left) {
    
        MadAnchor slideAnchor = MadTransform.FindChildWithName<MadAnchor>(transform, anchorName);
        if (slideAnchor != null) {
            DestroyImmediate(slideAnchor.gameObject);
        }
    
        slideAnchor = CreateChild<MadAnchor>(anchorName);
        if (hideManagedObjects) {
            slideAnchor.gameObject.hideFlags = HideFlags.HideInHierarchy;
        }
        
        slideAnchor.position = left ? MadAnchor.Position.Left : MadAnchor.Position.Right;
        slideAnchor.Update(); // explict update call because position has changed
        
        var offset = MadTransform.CreateChild(slideAnchor.transform, "Offset");
        offset.transform.localPosition =
            new Vector3(left ? template.texture.width / 2 : -template.texture.width / 2, 0, 0);
        
        var slide = MadTransform.CreateChild<MadSprite>(offset.transform, "slide", template);
        slide.transform.localScale = Vector3.one;
        slide.transform.localPosition = Vector3.zero;
        slide.guiDepth = 1000;
        
        return slide;
    }
    
    bool generate {
        get {
            return setupMethod == SetupMethod.Generate;
        }
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    private class Page {
        public int dragStopIndex { get; private set; }
        
        public Page(int dragStopIndex) {
            this.dragStopIndex = dragStopIndex;
        }
    }
    
    public enum SetupMethod {
        Generate,
        Manual,
    }

}

#if !UNITY_3_5
} // namespace
#endif