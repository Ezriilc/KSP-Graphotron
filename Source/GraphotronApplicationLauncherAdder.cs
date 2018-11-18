using KSP.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Graphotron.Source {
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    public class GraphotronApplicationLauncherAdder : MonoBehaviour﻿ {

        public void Awake() {
            Immortal.AddImmortal<GraphotronBehaviour>();
        }
    }

    public static class Immortal {
        private static GameObject _gameObject;

        public static T AddImmortal<T>() where T : Component {
            if (_gameObject == null) {
                _gameObject = new GameObject("GraphotronImmortal", typeof(T));
                UnityEngine.Object.DontDestroyOnLoad(_gameObject);
            }
            return _gameObject.GetComponent<T>() ?? _gameObject.AddComponent<T>();
        }
    }

    public class GraphotronBehaviour : MonoBehaviour {

        private static ApplicationLauncherButton appLauncherButton;

        public void Awake() {
            GameEvents.onGUIApplicationLauncherReady.Add(AddAppLauncher);
            GameEvents.onGUIApplicationLauncherDestroyed.Add(RemoveAppLauncher);
        }

        private void AddAppLauncher() {
            if (appLauncherButton != null) //button already added
                return;

            var applauncher = ApplicationLauncher.Instance;
            if (applauncher == null) //aplauncher is null
                return;

            var tex = new Texture2D(38, 38, TextureFormat.RGBA32, false);
            for (int x = 0; x < 38; x++) {
                for (int y = 0; y < 38; y++) {
                    tex.SetPixel(x, y, Color.clear);
                }
            }
            for (int x = 0; x < 26; x++) {
                for (int y = 0; y < 26; y++) {
                    if (x % 4 == 0 || y % 4 == 0)
                        tex.SetPixel(x + 6, y + 6, Color.white);
                }
            }
            tex.Apply();

            appLauncherButton = applauncher.AddModApplication(
                    ToggleGraphotronWindow,
                    ToggleGraphotronWindow,
                    () => { },
                    () => { },
                    () => { },
                    () => { },
                    ApplicationLauncher.AppScenes.FLIGHT,
                    tex
                );
        }

        private void ToggleGraphotronWindow() {

            bool wereAnyShown = false;
            foreach (var p in FindObjectsOfType<ModuleEnviroSensorPlotter>()) {
                if (p.isWindowShownMain) {
                    p.isWindowShownMain = false;
                    wereAnyShown = true;
                }
            }

            if (!wereAnyShown) {
                foreach (var part in FlightGlobals.ActiveVessel.Parts) {
                    foreach (var module in part.Modules) {
                        if (module is ModuleEnviroSensorPlotter) {
                            (module as ModuleEnviroSensorPlotter).isWindowShownMain = true;
                            return;
                        }
                    }
                }
                ScreenMessages.PostScreenMessage("There is no Graphotron module attatched to this craft.", false); //if no plotter was found
            }
        }

        private void RemoveAppLauncher() {

            if (appLauncherButton == null) //button already removed
                return;

            var applauncher = ApplicationLauncher.Instance;
            if (applauncher == null) //aplauncher is null
                return;

            applauncher.RemoveModApplication(appLauncherButton);
            appLauncherButton = null;
        }
    }
}
