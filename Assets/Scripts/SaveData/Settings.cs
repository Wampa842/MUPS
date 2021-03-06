﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

using UnityEngine;
using UnityEngine.UI;

namespace MUPS.SaveData
{
    /// <summary>
    /// A key binding consisting of a key and an optional modifier.
    /// </summary>
    public struct KeyBinding
    {
        /// <summary>
        /// Returns true if the specified key is a modifier key (Ctrl, Command, Shift, Alt, Windows, Menu).
        /// </summary>
        public static bool ValidModifier(KeyCode key)
        {
            switch (key)
            {
                case KeyCode.LeftShift:
                case KeyCode.RightShift:
                case KeyCode.LeftControl:
                case KeyCode.RightControl:
                case KeyCode.LeftCommand:
                case KeyCode.RightCommand:
                case KeyCode.AltGr:
                case KeyCode.LeftAlt:
                case KeyCode.RightAlt:
                case KeyCode.LeftWindows:
                case KeyCode.RightWindows:
                case KeyCode.Menu:
                    return true;
                default:
                    return false;
            }
        }

        public enum ModifierOperation { And, Or }

        /// <summary>
        /// The main key that activates the key binding if the modifier key is held down.
        /// </summary>
        public KeyCode Key;
        /// <summary>
        /// The first modifier key that allows the main key to activate the key binding.
        /// </summary>
        public KeyCode Modifier1;

        /// <summary>
        /// The second modifier key that allows the main key to activate the key binding.
        /// </summary>
        public KeyCode Modifier2;

        /// <summary>
        /// Determines whether either or both modifier keys need to be pressed.
        /// </summary>
        public ModifierOperation Operation;

        /// <summary>
        /// Creates a key binding with the specified main and modifier keys.
        /// </summary>
        public KeyBinding(KeyCode key, KeyCode modifier)
        {
            Key = key;
            Modifier1 = modifier;
            Modifier2 = KeyCode.None;
            Operation = ModifierOperation.Or;
        }

        /// <summary>
        /// Creates a key binding with the specified main and modifier keys.
        /// </summary>
        public KeyBinding(KeyCode key, KeyCode modifier1, KeyCode modifier2, ModifierOperation operation = ModifierOperation.Or)
        {
            Key = key;
            Modifier1 = modifier1;
            Modifier2 = modifier2;
            Operation = operation;
        }

        /// <summary>
        /// Creates a key binding with the specified main key and no modifiers.
        /// </summary>
        public KeyBinding(KeyCode key)
        {
            Key = key;
            Modifier1 = KeyCode.None;
            Modifier2 = KeyCode.None;
            Operation = ModifierOperation.Or;
        }

        /// <summary>
        /// Returns true during the frame in which the key is pressed, and if there's a modifier key, the modifier is held down.
        /// </summary>
        public bool Down()
        {
            if (Input.GetKeyDown(Key))
            {
                if (Modifier1 == KeyCode.None)
                {
                    return true;
                }
                else if (Modifier2 == KeyCode.None)
                {
                    return Input.GetKey(Modifier1);
                }
                else
                {
                    if (Operation == ModifierOperation.And)
                        return Input.GetKey(Modifier1) && Input.GetKey(Modifier2);
                    else
                        return Input.GetKey(Modifier1) || Input.GetKey(Modifier2);
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true during the frame in which the key is released, and if there's a modifier key, the modifier is held down.
        /// </summary>
        public bool Up()
        {
            if (Input.GetKeyUp(Key))
            {
                if (Modifier1 == KeyCode.None)
                {
                    return true;
                }
                else if (Modifier2 == KeyCode.None)
                {
                    return Input.GetKey(Modifier1);
                }
                else
                {
                    if (Operation == ModifierOperation.And)
                        return Input.GetKey(Modifier1) && Input.GetKey(Modifier2);
                    else
                        return Input.GetKey(Modifier1) || Input.GetKey(Modifier2);
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true while the main key (and optionally the modifier key) is held down.
        /// </summary>
        public bool Pressed()
        {
            if (Input.GetKey(Key))
            {
                if (Modifier1 == KeyCode.None)
                {
                    return true;
                }
                else if (Modifier2 == KeyCode.None)
                {
                    return Input.GetKey(Modifier1);
                }
                else
                {
                    if (Operation == ModifierOperation.And)
                        return Input.GetKey(Modifier1) && Input.GetKey(Modifier2);
                    else
                        return Input.GetKey(Modifier1) || Input.GetKey(Modifier2);
                }
            }
            return false;
        }
    }

    /// <summary>
    /// User input settings.
    /// </summary>
    [Serializable]
    public class KeyboardControls
    {
        // View controls
        public KeyCode SlowerModifier { get; private set; }
        public KeyCode FasterModifier { get; private set; }
        public KeyCode AltModifier { get; private set; }
        [JsonIgnore]
        public KeyCode Select { get; private set; }
        [JsonIgnore]
        public KeyCode RotateCamera { get; private set; }
        [JsonIgnore]
        public KeyCode PanCamera { get; private set; }

        // Manipulation controls
        public KeyBinding SelectObject { get; set; }
        public KeyBinding ToggleLocal { get; set; }
        public KeyBinding ToggleBoneVisible { get; set; }
        public KeyBinding ResetCamera { get; set; }
        public KeyBinding LoadCameraState { get; set; }
        public KeyBinding RegisterState { get; set; }
        public KeyBinding PreviousFrame { get; set; }
        public KeyBinding NextFrame { get; set; }
        public KeyBinding ToggleScreenGizmo { get; set; }

        // IO controls
        public KeyBinding Save { get; private set; }
        public KeyBinding Load { get; private set; }

        public KeyboardControls()
        {
            SlowerModifier = KeyCode.LeftControl;
            FasterModifier = KeyCode.LeftShift;
            AltModifier = KeyCode.LeftAlt;
            Select = KeyCode.Mouse0;
            RotateCamera = KeyCode.Mouse1;
            PanCamera = KeyCode.Mouse2;

            SelectObject = new KeyBinding(KeyCode.Mouse0, KeyCode.LeftControl);
            ToggleLocal = new KeyBinding(KeyCode.G);
            ToggleBoneVisible = new KeyBinding(KeyCode.C);
            ResetCamera = new KeyBinding(KeyCode.R, KeyCode.LeftControl, KeyCode.RightControl);
            LoadCameraState = new KeyBinding(KeyCode.R, KeyCode.LeftShift, KeyCode.RightShift);
            RegisterState = new KeyBinding(KeyCode.Return);
            PreviousFrame = new KeyBinding(KeyCode.PageUp);
            NextFrame = new KeyBinding(KeyCode.PageDown);
            ToggleScreenGizmo = new KeyBinding(KeyCode.X);

            Save = new KeyBinding(KeyCode.S);
            Load = new KeyBinding(KeyCode.L);
        }
    }

    /// <summary>
    /// Scene display settings.
    /// </summary>
    [Serializable]
    public class ViewProperties
    {
        public float BoneSize { get; set; }
        public float BoneTailSize { get; set; }
        public float AxisIndicatorSize { get; set; }
        public float BoneOpacity { get; set; }
        public float BoneTailOpacity { get; set; }

        public ViewProperties()
        {
            BoneSize = 0.025f;
            BoneTailSize = 0.015f;
            AxisIndicatorSize = 0.35f;
            BoneOpacity = 0.7f;
            BoneTailOpacity = 0.4f;
        }
    }

    /// <summary>
    /// Application and system settings.
    /// </summary>
    [Serializable]
    public class ApplicationProperties
    {
        public Log.LogLevel MinimumLogLevel { get; set; }
        public int RenderWidth { get; set; }
        public int RenderHeight { get; set; }
        public string LastRenderPath { get; set; }
        public string LastModelPath { get; set; }

        public ApplicationProperties()
        {
            MinimumLogLevel = Log.LogLevel.Trace;
            RenderWidth = 1920;
            RenderHeight = 1080;
            LastRenderPath = Path.Combine(Application.persistentDataPath, "image.png");
            LastModelPath = Application.persistentDataPath;
        }
    }

    [Serializable]
    class Settings
    {
        public KeyboardControls Keyboard { get; set; }
        public ViewProperties View { get; set; }
        public ApplicationProperties ApplicationSettings { get; set; }

        public static Settings Current { get; set; } = new Settings();
        public static string SettingsFile { get { return Path.Combine(Application.persistentDataPath, "config.json"); } }

        public Settings()
        {
            Keyboard = new KeyboardControls();
            View = new ViewProperties();
            ApplicationSettings = new ApplicationProperties();
        }

        // Save settings into a specified file
        public static void Export(string path)
        {
            StreamWriter stream = null;
            JsonWriter writer = null;
            JsonSerializer serializer = new JsonSerializer { NullValueHandling = NullValueHandling.Ignore };

            try
            {
                stream = new StreamWriter(path);
                writer = new JsonTextWriter(stream);
                serializer.Serialize(writer, Current);
                Log.Info("Config saved.");
            }
            catch (Exception ex)
            {
                Log.Error("An exception occured while trying to save the application settings.");
                Log.Error(ex);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (writer != null)
                    writer.Close();
            }
        }

        // Save settings into the config file
        public static void Save()
        {
            Export(SettingsFile);
        }

        // Load settings from a specified file
        public static void Import(string path)
        {
            StreamReader stream = null;
            JsonReader reader = null;
            JsonSerializer serializer = new JsonSerializer { NullValueHandling = NullValueHandling.Ignore };

            try
            {
                if (!File.Exists(path))
                {
                    Current = new Settings();
                    Save();
                    Log.Warning("Config file not found - default config loaded.");
                }
                else
                {
                    stream = new StreamReader(path);
                    reader = new JsonTextReader(stream);
                    Current = serializer.Deserialize<Settings>(reader);
                    Log.Info("Config loaded.");
                }
            }
            catch (Exception ex)
            {
                Log.Error("An exception occured while trying to load the application settings.");
                Log.Error(ex);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (reader != null)
                    reader.Close();
            }
        }

        // Load settings from the config file
        public static void Load()
        {
            Import(SettingsFile);
        }
    }
}
