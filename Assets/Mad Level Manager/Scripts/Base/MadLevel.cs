/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MadLevelManager {

/// <summary>
/// Level access & loading routines.
/// </summary>
public class MadLevel  {

    // ===========================================================
    // Constants
    // ===========================================================
    
    // ===========================================================
    // Fields
    // ===========================================================
    
    static MadLevelConfiguration _configuration;
    static string _arguments = null;
    static string _currentLevelName = null;
    
    /// <summary>
    /// Gets the active configuration.
    /// </summary>
    /// <value>
    /// The active configuration.
    /// </value>
    public static MadLevelConfiguration activeConfiguration {
        get {
            if (_configuration == null || !_configuration.active) {
                _configuration = MadLevelConfiguration.GetActive();
            }
            
            return _configuration;
        }
    }
    
    /// <summary>
    /// Gets a value indicating whether application has active configuration.
    /// </summary>
    /// <value>
    /// <c>true</c> if has active configuration; otherwise, <c>false</c>.
    /// </value>
    public static bool hasActiveConfiguration {
        get {
            if (_configuration == null) {
                    var configurations = Resources.LoadAll(
                        "LevelConfig", typeof(MadLevelConfiguration));
                    return configurations.Length > 0;
            } else {
                return true;
            }
        }
    }
    
    /// <summary>
    /// Gets or sets this level arguments.
    /// </summary>
    /// <value>
    /// The arguments.
    /// </value>
    public static string arguments {
        get {
            if (_arguments == null) {
                FindCurrentSceneLevel();
            }
        
            return _arguments;
        }
        
        set {
            _arguments = value;
        }
    }
    
    /// <summary>
    /// Gets the name of the current level.
    /// </summary>
    /// <value>
    /// The name of the current level.
    /// </value>
    public static string currentLevelName {
        get {
            if (_currentLevelName == null) {
                FindCurrentSceneLevel();
            }
        
            return _currentLevelName;
        }
        
        set {
            _currentLevelName = value;
        }
    }
    
    static void FindCurrentSceneLevel() {
        // find first level with matching scene name
        var level = activeConfiguration.FindFirstForScene(Application.loadedLevelName);
        if (level != null) {
            currentLevelName = level.name;
            arguments = level.arguments;
            
            Debug.Log("Mad Level Manager: This was first scene opened. Assuming that this was '"
                + _currentLevelName + "' level. http://goo.gl/9sDjSi");
        } else {
            Debug.LogError("Mad Level Manager: Cannot find scene " + Application.loadedLevelName
                + " in the configuration. Is the level configuration broken or wrong configuration is active?");
            currentLevelName = "";
            arguments = "";
        }
    }
    
    [System.Obsolete("Use lastPlayedLevelName instead.")]
    public static string lastLevelName {
        get {
            return lastPlayedLevelName;
        }
        
        set {
            lastPlayedLevelName = value;
        }
    }
    
    /// <summary>
    /// Gets or sets the name of the last visited level.
    /// </summary>
    /// <value>
    /// The name of the last visited level.
    /// </value>
    public static string lastPlayedLevelName {
        get; set;
    }

    // ===========================================================
    // Methods
    // ===========================================================
    
    /// <summary>
    /// Reloads current level.
    /// </summary>
    public static void ReloadCurrent() {
        Application.LoadLevel(Application.loadedLevel);
    }
    
    /// <summary>
    /// Reloads the current level asynchronously in the background. This function requires Unity Pro.
    /// </summary>
    /// <returns>AsyncOperation object.</returns>
    public static AsyncOperation ReloadCurrentAsync() {
        return Application.LoadLevelAsync(Application.loadedLevel);
    }
    
    /// <summary>
    /// Loads level by its name defined in level configuration.
    /// </summary>
    /// <param name='levelName'>
    /// Level name.
    /// </param>
    public static void LoadLevelByName(string levelName) {
        CheckHasConfiguration();
        var level = activeConfiguration.FindLevelByName(levelName);
        if (level != null) {
            LoadLevel(level);
        } else {
            Debug.LogError(string.Format("Level \"{0}\" not found. Please verify your configuration.", levelName));
        }
    }
    
    /// <summary>
    /// Loads level by its name defined in level configuration asynchronously in the background.
    /// This function requires Unity Pro.
    /// </summary>
    /// <param name='levelName'>
    /// Level name.
    /// </param>
    /// <returns>AsyncOperation object.</returns>
    public static AsyncOperation LoadLevelByNameAsync(string levelName) {
        CheckHasConfiguration();
        var level = activeConfiguration.FindLevelByName(levelName);
        if (level != null) {
            return LoadLevelAsync(level);
        } else {
            Debug.LogError(string.Format("Level \"{0}\" not found. Please verify your configuration.", levelName));
            return null;
        }
    }
    
    /// <summary>
    /// Determines whether there is next level present in level configuration
    /// (are there any other levels further ahead?)
    /// </summary>
    /// <returns>
    /// <c>true</c> if next level is available; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasNext() {
        CheckHasConfiguration();
        return activeConfiguration.FindNextLevel(currentLevelName) != null;
    }
    
    /// <summary>
    /// Determines whether there is next level present of the specified levelType in level configuration
    /// (are there any other levels further ahead?)
    /// </summary>
    /// <returns>
    /// <c>true</c> if next level is present; otherwise, <c>false</c>.
    /// </returns>
    /// <param name='levelType'>
    /// Type of next level to look after.
    /// </param>
    public static bool HasNext(Type levelType) {
        CheckHasConfiguration();
        return activeConfiguration.FindNextLevel(currentLevelName, levelType) != null;
    }
    
    /// <summary>
    /// Loads next level defined in level configuration.
    /// </summary>
    public static void LoadNext() {
        CheckHasConfiguration();
        var nextLevel = activeConfiguration.FindNextLevel(currentLevelName);
        if (nextLevel != null) {
            LoadLevel(nextLevel);
        } else {
            Debug.LogError("Cannot load next level: This is the last level.");
        }
    }
    
    /// <summary>
    /// Loads next level defined in level configuration asynchronously in the background.
    /// This function requires Unity Pro.
    /// </summary>
    /// <returns>AsyncOperation object.</returns>
    public static AsyncOperation LoadNextAsync() {
        CheckHasConfiguration();
        var nextLevel = activeConfiguration.FindNextLevel(currentLevelName);
        if (nextLevel != null) {
            return LoadLevelAsync(nextLevel);
        } else {
            Debug.LogError("Cannot load next level: This is the last level.");
            return null;
        }
    }
    
    /// <summary>
    /// Loads first level with type <code>levelType</code> found after current level in level configuration.
    /// </summary>
    /// <param name='levelType'>
    /// Level type to load.
    /// </param>
    public static void LoadNext(Type levelType) {
        CheckHasConfiguration();
        var nextLevel = activeConfiguration.FindNextLevel(currentLevelName, levelType);
        if (nextLevel != null) {
            LoadLevel(nextLevel);
        } else {
            Debug.LogError("Cannot load next level: This is the last level of requested type.");
        }
    }
    
    /// <summary>
    /// Loads first level with type <code>levelType</code> found after current level in level configuration 
    /// asynchronously in the background. This function requires Unity Pro.
    /// </summary>
    /// <param name='levelType'>
    /// Level type to load.
    /// </param>
    /// <returns>AsyncOperation object.</returns>
    public static AsyncOperation LoadNextAsync(Type levelType) {
        CheckHasConfiguration();
        var nextLevel = activeConfiguration.FindNextLevel(currentLevelName, levelType);
        if (nextLevel != null) {
            return LoadLevelAsync(nextLevel);
        } else {
            Debug.LogError("Cannot load next level: This is the last level of requested type.");
            return null;
        }
    }
    
    /// <summary>
    /// Determines whether there is previous level present in level configuration
    /// (are there any other levels before this one?)
    /// </summary>
    /// <returns>
    /// <c>true</c> if previous level is available; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasPrevious() {
        CheckHasConfiguration();
        return activeConfiguration.FindPreviousLevel(currentLevelName) != null;
    }
    
    /// <summary>
    /// Determines whether there is previous level present of the specified levelType in level configuration
    /// (are there any other levels further ahead?)
    /// </summary>
    /// <returns>
    /// <c>true</c> if previous level is present; otherwise, <c>false</c>.
    /// </returns>
    /// <param name='levelType'>
    /// Type of previous level to look after.
    /// </param>
    public static bool HasPrevious(Type levelType) {
        CheckHasConfiguration();
        return activeConfiguration.FindPreviousLevel(currentLevelName, levelType) != null;
    }
    
    /// <summary>
    /// Loads previous level defined in level configuration.
    /// </summary>
    public static void LoadPrevious() {
        CheckHasConfiguration();
        var previousLevel = activeConfiguration.FindPreviousLevel(currentLevelName);
        if (previousLevel != null) {
            LoadLevel(previousLevel);
        } else {
            Debug.LogError("Cannot load previous level: This is the first level.");
        }
    }
    
    /// <summary>
    /// Loads previous level defined in level configuration asynchronously in the background.
    /// This function requires Unity Pro.
    /// </summary>
    /// <returns>AsyncOperation object.</returns>
    public static AsyncOperation LoadPreviousAsync() {
        CheckHasConfiguration();
        var previousLevel = activeConfiguration.FindPreviousLevel(currentLevelName);
        if (previousLevel != null) {
            return LoadLevelAsync(previousLevel);
        } else {
            Debug.LogError("Cannot load previous level: This is the first level.");
            return null;
        }
    }
    
    /// <summary>
    /// Loads first level with type <code>levelType</code> found before current level in level configuration.
    /// </summary>
    /// <param name='levelType'>
    /// Level type to load.
    /// </param>
    public static void LoadPrevious(Type levelType) {
        CheckHasConfiguration();
        var previousLevel = activeConfiguration.FindPreviousLevel(currentLevelName, levelType);
        if (previousLevel != null) {
            LoadLevel(previousLevel);
        } else {
            Debug.LogError("Cannot load previous level: This is the first level of requested type.");
        }
    }
    
    /// <summary>
    /// Loads first level with type <code>levelType</code> found before current level in level configuration
    /// asynchronously in the background. This function requires Unity Pro.
    /// </summary>
    /// <param name='levelType'>
    /// Level type to load.
    /// </param>
    /// <returns>AsyncOperation object.</returns>
    public static AsyncOperation LoadPreviousAsync(Type levelType) {
        CheckHasConfiguration();
        var previousLevel = activeConfiguration.FindPreviousLevel(currentLevelName, levelType);
        if (previousLevel != null) {
            return LoadLevelAsync(previousLevel);
        } else {
            Debug.LogError("Cannot load previous level: This is the first level of requested type.");
            return null;
        }
    }
    
    /// <summary>
    /// Tells if there is at least one level set in active level configuration.
    /// </summary>
    /// <returns>
    /// <c>true</c> if there is at least one level configured; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasFirst() {
        return activeConfiguration.LevelCount() != 0;
    }
    
    /// <summary>
    /// Tells if there is at least one level of type <code>levelType</code> set in active level configuration.
    /// </summary>
    /// <param name='levelType'>
    /// Level type to find.
    /// </param>
    /// <returns>
    /// <c>true</c> if there is at least one level of given type configured; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasFirst(Type levelType) {
        return activeConfiguration.LevelCount(levelType) != 0;
    }
    
    /// <summary>
    /// Loads the first level set in level configuration.
    /// </summary>
    public static void LoadFirst() {
        if (activeConfiguration.LevelCount() != 0) {
            var firstLevel = activeConfiguration.GetLevel(0);
            LoadLevel(firstLevel);
        } else {
            Debug.LogError("Cannot load first level: there's no levels defined");
        }
    }
    
    /// <summary>
    /// Loads the first level set in level configuration asynchronously in the background.
    /// This function requires Unity Pro.
    /// </summary>
    /// <returns>AsyncOperation object.</returns>
    public static AsyncOperation LoadFirstAsync() {
        if (activeConfiguration.LevelCount() != 0) {
            var firstLevel = activeConfiguration.GetLevel(0);
            return LoadLevelAsync(firstLevel);
        } else {
            Debug.LogError("Cannot load first level: there's no levels defined");
            return null;
        }
    }
    
    /// <summary>
    /// Loads the first level of type <code>levelType</code> set in level configuration.
    /// </summary>
    /// <param name='levelType'>
    /// Level type.
    /// </param>
    public static void LoadFirst(Type levelType) {
        if (activeConfiguration.LevelCount(levelType) != 0) {
            var firstLevel = activeConfiguration.GetLevel(levelType, 0);
            LoadLevel(firstLevel);
        } else {
            Debug.LogError("Cannot load first level: there's no level of type " + levelType);
        }
    }
    
    /// <summary>
    /// Loads the first level of type <code>levelType</code> set in level configuration asynchronously in the
    /// background. This function requires Unity Pro.
    /// </summary>
    /// <param name='levelType'>
    /// Level type.
    /// </param>
    /// <returns>AsyncOperation object.</returns>
    public static AsyncOperation LoadFirstAsync(Type levelType) {
        if (activeConfiguration.LevelCount(levelType) != 0) {
            var firstLevel = activeConfiguration.GetLevel(levelType, 0);
            return LoadLevelAsync(firstLevel);
        } else {
            Debug.LogError("Cannot load first level: there's no level of type " + levelType);
            return null;
        }
    }
    
    /// <summary>
    /// Gets the all defined level names.
    /// </summary>
    /// <returns>The all defined level names.</returns>
    public static string[] GetAllLevelNames() {
        CheckHasConfiguration();
        string[] result = new string[activeConfiguration.levels.Count];
        for (int i = 0; i < activeConfiguration.levels.Count; ++i) {
            result[i] = activeConfiguration.levels[i].name;
        }
        
        return result;
    }
    
    /// <summary>
    /// Gets the all defined level names of given type.
    /// </summary>
    /// <returns>The all defined level names of given type.</returns>
    public static string[] GetAllLevelNames(MadLevel.Type type) {
        CheckHasConfiguration();
        var output = new List<string>();
        for (int i = 0; i < activeConfiguration.levels.Count; ++i) {
            var level = activeConfiguration.levels[i];
            if (level.type == type) {
                output.Add(level.name);
            }
        }
        
        return output.ToArray();
    }
    
    /// <summary>
    /// Finds the first completed level (in the order).
    /// </summary>
    /// <returns>The first completed level name or <code>null</code> if there's no level
    /// that is marked as completed.</returns>
    public static string FindFirstCompletedLevelName() {
        return FindFirstLevelName((level) => MadLevelProfile.IsCompleted(level.name));
    }
    
    /// <summary>
    /// Finds the last completed level (in the order).
    /// </summary>
    /// <returns>The last completed level name or <code>null</code> if there's no level
    /// that is marked as completed.</returns>
    public static string FindLastCompletedLevelName() {
        return FindLastLevelName((level) => MadLevelProfile.IsCompleted(level.name));
    }
    
    /// <summary>
    /// Finds the first locked level (in the order). Be aware that locked flag in the new
    /// game is set when the player visits level select screen for the first time.
    /// </summary>
    /// <returns>The first locked level name or <code>null</code> if there's no level
    /// that is marked as locked.</returns>
    public static string FindFirstLockedLevelName() {
        return FindFirstLevelName((level) => MadLevelProfile.IsLocked(level.name));
    }
    
    /// <summary>
    /// Finds the last locked level (in the order). Be aware that locked flag in the new
    /// game is set when the player visits level select screen for the first time.
    /// </summary>
    /// <returns>The last locked level name or <code>null</code> if there's no level
    /// that is marked as locked.</returns>
    public static string FindLastLockedLevelName() {
        return FindLastLevelName((level) => MadLevelProfile.IsLocked(level.name));
    }
    
    /// <summary>
    /// Finds the first unlocked level (in the order). Be aware that locked flag in the new
    /// game is set when the player visits level select screen for the first time.
    /// </summary>
    /// <returns>The first locked level name or <code>null</code> if there's no level
    /// that is marked as locked.</returns>
    public static string FindFirstUnlockedLevelName() {
        return FindFirstLevelName((level) => !MadLevelProfile.IsLocked(level.name));
    }
    
    /// <summary>
    /// Finds the last unlocked level (in the order). Be aware that locked flag in the new
    /// game is set when the player visits level select screen for the first time.
    /// </summary>
    /// <returns>The first locked level name or <code>null</code> if there's no level
    /// that is marked as locked.</returns>
    public static string FindLastUnlockedLevelName() {
        return FindLastLevelName((level) => !MadLevelProfile.IsLocked(level.name));
    }
    
    /// <summary>
    /// Finds the first name of the level, that predicate returns <code>true</code> value.
    /// </summary>
    /// <returns>The first found level or <code>null<code> if not found.</returns>
    /// <param name="predicate">The predicate.</param>
    public static string FindFirstLevelName(LevelPredicate predicate) {
        CheckHasConfiguration();
        
        var levels = activeConfiguration.GetLevelsInOrder();
        for (int i = 0; i < levels.Length; i++) {
            var level = levels[i];
            if (predicate(level)) {
                return level.name;
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Finds the first name of the level of given <code>type</code>.
    /// </summary>
    /// <returns>The first found level or <code>null<code> if not found.</returns>
    /// <param name="levelType">The level type.</param>
    public static string FindFirstLevelName(MadLevel.Type levelType) {
        return FindFirstLevelName((level) => level.type == levelType);
    }
    
    /// <summary>
    /// Finds the last name of the level, that predicate returns <code>true</code> value.
    /// </summary>
    /// <returns>The last found level or <code>null<code> if not found.</returns>
    /// <param name="predicate">The predicate.</param>
    public static string FindLastLevelName(LevelPredicate predicate) {
        CheckHasConfiguration();
    
        var levels = activeConfiguration.GetLevelsInOrder();
        for (int i = levels.Length - 1; i >= 0; i--) {
            var level = levels[i];
            if (predicate(level)) {
                return level.name;
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Finds the last name of the level of given <code>type</code>.
    /// </summary>
    /// <returns>The last found level or <code>null<code> if not found.</returns>
    /// <param name="levelType">The level type.</param>
    public static string FindLastLevelName(MadLevel.Type levelType) {
        return FindLastLevelName((level) => level.type == levelType);
    }
    
    /// <summary>
    /// Gets the name of the previous level.
    /// </summary>
    /// <returns>The previous level name or <code>null<code> if not found.</returns>
    public static string GetPreviousLevelName() {
        CheckHasConfiguration();
        
        var level = activeConfiguration.FindPreviousLevel(currentLevelName);
        return level != null ? level.name : null;
    }
    
    /// <summary>
    /// Gets the name of the previous level of given type.
    /// </summary>
    /// <returns>The previous level name or <code>null</code> if not found.</returns>
    /// <param name="type">Level type.</param>
    public static string GetPreviousLevelName(MadLevel.Type type) {
        CheckHasConfiguration();
        
        var level = activeConfiguration.FindPreviousLevel(currentLevelName, type);
        return level != null ? level.name : null;
    }
    
    /// <summary>
    /// Gets the name of a level that exists before given level.
    /// </summary>
    /// <returns>The name of previous level or <code>null</code> if not found.</returns>
    /// <param name="levelName">Level name.</param>
    public static string GetPreviousLevelNameTo(string levelName) {
        CheckHasConfiguration();
        CheckLevelExists(levelName);
        
        var level = activeConfiguration.FindPreviousLevel(levelName);
        return level != null ? level.name : null;
    }
    
    /// <summary>
    /// Gets the name of a level (with given type) that exists before given level.
    /// </summary>
    /// <returns>The name of previous level or <code>null</code> if not found.</returns>
    /// <param name="levelName">Level name.</param>
    /// <param name="type">Type of previous level.</param>
    public static string GetPreviousLevelNameTo(string levelName, MadLevel.Type type) {
        CheckHasConfiguration();
        CheckLevelExists(levelName);
        
        var level = activeConfiguration.FindPreviousLevel(levelName, type);
        return level != null ? level.name : null;
    }
    
    /// <summary>
    /// Gets the name of the next level.
    /// </summary>
    /// <returns>The next level name or <code>null</code> if not found.</returns>
    public static string GetNextLevelName() {
        CheckHasConfiguration();
        
        var level = activeConfiguration.FindNextLevel(currentLevelName);
        return level != null ? level.name : null;
    }
    
    /// <summary>
    /// Gets the name of the next level of given type.
    /// </summary>
    /// <returns>The next level name or <code>null</code> if not found.</returns>
    /// <param name="type">Level type.</param>
    public static string GetNextLevelName(MadLevel.Type type) {
        CheckHasConfiguration();
        
        var level = activeConfiguration.FindNextLevel(currentLevelName, type);
        return level != null ? level.name : null;
    }
    
    /// <summary>
    /// Gets the name of a level that exists after given level.
    /// </summary>
    /// <returns>The name of next level or <code>null</code> if not found.</returns>
    /// <param name="levelName">Level name.</param>
    public static string GetNextLevelNameTo(string levelName) {
        CheckHasConfiguration();
        CheckLevelExists(levelName);
        
        var level = activeConfiguration.FindNextLevel(levelName);
        return level != null ? level.name : null;
    }
    
    /// <summary>
    /// Gets the name of a level (with given type) that exists after given level.
    /// </summary>
    /// <returns>The name of next level or <code>null</code> if not found.</returns>
    /// <param name="levelName">Level name.</param>
    /// <param name="type">Type of previous level.</param>
    public static string GetNextLevelNameTo(string levelName, MadLevel.Type type) {
        CheckHasConfiguration();
        CheckLevelExists(levelName);
        
        var level = activeConfiguration.FindNextLevel(levelName, type);
        return level != null ? level.name : null;
    }
    
    static void CheckLevelExists(string levelName) {
        var level = activeConfiguration.FindLevelByName(levelName);
        MadDebug.Assert(level != null, "There's no level with name '" + levelName + "'");
    }
    
    static void CheckHasConfiguration() {
        MadDebug.Assert(hasActiveConfiguration,
            "This method may only be used when level configuration is set. Please refer to "
            + MadLevelHelp.ConfigurationWiki);
    }
    
    static void LoadLevel(MadLevelConfiguration.Level level) {
        lastPlayedLevelName = currentLevelName;
        // arguments must be set after reading currentLevelName
        // because reading it may overwrite arguments in result
        // TODO: find a better way to solve this
        arguments = level.arguments;
        currentLevelName = level.name;
        Application.LoadLevel(level.sceneName); // TODO: change it to scene index
    }
    
    static AsyncOperation LoadLevelAsync(MadLevelConfiguration.Level level) {
        lastPlayedLevelName = currentLevelName;
        // arguments must be set after reading currentLevelName
        // because reading it may overwrite arguments in result
        // TODO: find a better way to solve this
        arguments = level.arguments;
        currentLevelName = level.name;
        return Application.LoadLevelAsync(level.sceneName); // TODO: change it to scene index
    }
    
    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    public enum Type {
        Other,
        Level,
        Extra,
    }
    
    public delegate bool LevelPredicate(MadLevelConfiguration.Level level);

}

} // namespace