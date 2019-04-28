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
                Log.Debug($"Cannot load frame at {SelectedFrame} - out of range.");
                return;
            }

            Frame frame = Frames[SelectedFrame];

            if (frame == null)
            {
                Log.Debug($"Cannot load frame at {SelectedFrame} - data is null.");
                return;
            }

            frame.Apply();

            Log.Debug($"Loaded frame {SelectedFrame}");
        }

        public void RegisterFrame()
        {
            if(SelectedFrame == Frames.Count)
            {
                Frames.Add(Frame.GetCurrent());
                Log.Debug($"Added new frame at {SelectedFrame}");
            }
            else
            {
                Frames[SelectedFrame] = Frame.GetCurrent();
                Log.Debug($"Overwrote frame at {SelectedFrame}");
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