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
        public Image FrameDisplay;

        // Assets
        public Sprite[] Sprites;

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
            UpdateFrameImage();
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
            if (SelectedFrame == Frames.Count)
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
            UpdateFrameImage();
        }

        public void UpdateFrameImage()
        {
            // None
            if (Frames.Count <= 0)
            {
                FrameDisplay.sprite = Sprites[5];
            }
            // One or more
            else
            {
                // First selected
                if(SelectedFrame == 0)
                {
                    // Only one
                    if(Frames.Count == 1)
                    {
                        FrameDisplay.sprite = Sprites[0];
                    }
                    // More than one
                    else
                    {
                        FrameDisplay.sprite = Sprites[1];
                    }
                }
                // New selected
                else if (SelectedFrame == Frames.Count)
                {
                    FrameDisplay.sprite = Sprites[4];
                }
                // Last selected
                else if(SelectedFrame == Frames.Count - 1)
                {
                    FrameDisplay.sprite = Sprites[3];
                }
                // Middle selected
                else
                {
                    FrameDisplay.sprite = Sprites[2];
                }
            }
        }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
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