using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace PracticePrototype
{
    public sealed class PracticeGameBootstrap : MonoBehaviour
    {
        [SerializeField]
        private PracticeUIController uiController;

        [SerializeField]
        private List<PracticeLevelEntry> levelEntries = new()
        {
            new PracticeLevelEntry
            {
                id = PracticeLevelId.Level1,
                route = "level1",
                title = "Level 1 - Algorithms",
                description = "Build a command sequence and guide the robot through a simple grid."
            },
            new PracticeLevelEntry
            {
                id = PracticeLevelId.Level2,
                route = "level2",
                title = "Level 2 - Variables",
                description = "Collect objects that match the requested Python data type or value."
            },
            new PracticeLevelEntry
            {
                id = PracticeLevelId.Level3,
                route = "level3",
                title = "Level 3 - Conditions",
                description = "Read the environment and choose the correct action using if-logic."
            },
            new PracticeLevelEntry
            {
                id = PracticeLevelId.Level4,
                route = "level4",
                title = "Level 4 - Loops",
                description = "Use repeat blocks to solve a repeated route more compactly."
            }
        };

        private PracticeUIController _ui;
        private Sprite _whiteSprite;
        private Font _defaultFont;
        private IPracticeLevel _currentLevel;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;

            _defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (_defaultFont == null)
            {
                _defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }
            _whiteSprite = CreateWhiteSprite();
            _ui = uiController != null ? uiController : FindFirstObjectByType<PracticeUIController>();
            if (_ui == null)
            {
                Debug.LogError("PracticeUIController was not found in scene WebGLPractice.");
                enabled = false;
                return;
            }

            ConfigureEventSystem();
            _ui.Build(_whiteSprite, _defaultFont);
            _ui.BindNavigation(HandleRun, HandleReset, ShowSelector);
        }

        private void Start()
        {
            var requestedLevel = ParseLevelFromUrl(Application.absoluteURL);
            if (requestedLevel == PracticeLevelId.None)
            {
                ShowSelector();
                return;
            }

            LoadLevel(requestedLevel);
        }

        private void Update()
        {
            _currentLevel?.Tick();
        }

        private void OnDestroy()
        {
            _currentLevel?.Dispose();
            if (_whiteSprite != null)
            {
                Destroy(_whiteSprite.texture);
                Destroy(_whiteSprite);
            }
        }

        public void ShowResult(PracticeResultData result)
        {
            _ui.ResultPopup.Show(result, HandleReset, ShowSelector);
        }

        public void HideResult()
        {
            _ui.ResultPopup.Hide();
        }

        private void HandleRun()
        {
            HideResult();
            _currentLevel?.RunLevel();
        }

        private void HandleReset()
        {
            HideResult();
            _currentLevel?.ResetLevel();
        }

        private void ShowSelector()
        {
            HideResult();
            _currentLevel?.Hide();
            _ui.BindNavigation(null, null, null);
            _ui.SetHeader("Python Practice Prototype", "Pick one lightweight 2D WebGL practice or open it directly by route.");
            _ui.SetStatus("Level selector is open.", PracticeUiFactory.TextSecondary);
            _ui.SetRunLabel("Run");
            _ui.SetRunInteractable(false);
            _ui.ShowSelector(levelEntries, LoadLevel);
        }

        private void LoadLevel(PracticeLevelId levelId)
        {
            HideResult();
            _ui.HideSelector();
            _currentLevel?.Dispose();
            _ui.ClearContent();
            _currentLevel = CreateLevel(levelId);

            if (_currentLevel == null)
            {
                ShowSelector();
                return;
            }

            var context = new PracticeRuntimeContext
            {
                Bootstrap = this,
                UI = _ui,
                WhiteSprite = _whiteSprite,
                DefaultFont = _defaultFont,
                ShowResult = ShowResult,
                ShowSelector = ShowSelector,
                LoadLevel = LoadLevel
            };

            _currentLevel.Build(context);
            _currentLevel.Show();
            _ui.SetHeader(_currentLevel.Title, _currentLevel.Description);
            _ui.SetRunLabel(levelId == PracticeLevelId.Level2 ? "Check" : "Run");
            _ui.SetRunInteractable(true);
        }

        private static Sprite CreateWhiteSprite()
        {
            var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.HideAndDontSave
            };
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        }

        private static void ConfigureEventSystem()
        {
            var eventSystem = FindFirstObjectByType<EventSystem>();
            if (eventSystem == null)
            {
                Debug.LogError("EventSystem is missing in scene WebGLPractice.");
                return;
            }

#if ENABLE_INPUT_SYSTEM
            var inputModule = eventSystem.GetComponent<InputSystemUIInputModule>();
            if (inputModule != null)
            {
                inputModule.AssignDefaultActions();
            }
#endif
        }

        private IPracticeLevel CreateLevel(PracticeLevelId levelId)
        {
            return levelId switch
            {
                PracticeLevelId.Level1 => new AlgorithmSequenceLevel(),
                PracticeLevelId.Level2 => new VariablesCollectorLevel(),
                PracticeLevelId.Level3 => new ConditionsRuleLevel(),
                PracticeLevelId.Level4 => new LoopsRepeatLevel(),
                _ => null
            };
        }

        private PracticeLevelId ParseLevelFromUrl(string absoluteUrl)
        {
            if (string.IsNullOrWhiteSpace(absoluteUrl))
            {
                return PracticeLevelId.None;
            }

            if (!Uri.TryCreate(absoluteUrl, UriKind.Absolute, out var uri))
            {
                return PracticeLevelId.None;
            }

            var candidate = uri.Segments.Length > 0 ? uri.Segments[^1].Trim('/').ToLowerInvariant() : string.Empty;
            if (string.IsNullOrWhiteSpace(candidate) || candidate.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
            {
                candidate = string.Empty;
            }

            if (string.IsNullOrWhiteSpace(candidate))
            {
                var query = uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries);
                foreach (var entry in query)
                {
                    var pair = entry.Split('=', 2, StringSplitOptions.RemoveEmptyEntries);
                    if (pair.Length == 2 && pair[0].Equals("level", StringComparison.OrdinalIgnoreCase))
                    {
                        candidate = pair[1].Trim().ToLowerInvariant();
                        break;
                    }
                }
            }

            foreach (var entry in levelEntries)
            {
                if (string.Equals(entry.route, candidate, StringComparison.OrdinalIgnoreCase))
                {
                    return entry.id;
                }
            }

            return PracticeLevelId.None;
        }
    }
}
