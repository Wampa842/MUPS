using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MUPS;

namespace MUPS.Scene
{
    public class FrameController : MonoBehaviour
    {
        // Singleton
        public static FrameController Instance { get; private set; }

        // Object references
        public Text FrameCountLabel;
        public Text SelectedFrameLabel;

        // Frames
        public List<Frame> Frames { get; private set; }
        public int SelectedFrame = 0;

        public void StepFrame(int step)
        {
            SelectedFrame += step;
            if (SelectedFrame > Frames.Count)
                SelectedFrame = Frames.Count;
            else if (SelectedFrame < 0)
                SelectedFrame = 0;

            SelectedFrameLabel.text = SelectedFrame.ToString();
        }

        public void LoadFrame()
        {
            if (SelectedFrame >= Frames.Count)
            {
                Logger.Log($"Cannot load frame at {SelectedFrame} - out of range.", Logger.LogLevel.Debug);
                return;
            }

            Frame frame = Frames[SelectedFrame];

            if (frame == null)
            {
                Logger.Log($"Cannot load frame at {SelectedFrame} - data is null.", Logger.LogLevel.Debug);
                return;
            }

            frame.Apply();

            Logger.Log($"Loaded frame {SelectedFrame}", Logger.LogLevel.Debug);
        }

        public void RegisterFrame()
        {
            if(SelectedFrame == Frames.Count)
            {
                Frames.Add(Frame.GetCurrent());
                Logger.Log($"Added new frame at {SelectedFrame}", Logger.LogLevel.Debug);
            }
            else
            {
                Frames[SelectedFrame] = Frame.GetCurrent();
                Logger.Log($"Overwrote frame at {SelectedFrame}", Logger.LogLevel.Debug);
            }
            FrameCountLabel.text = Frames.Count.ToString();
        }

        void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else if(Instance != this)
            {
                Destroy(Instance);
                Instance = this;
            }
        }

        void Start()
        {
            Frames = new List<Frame>();
            StepFrame(0);
        }
    }
}