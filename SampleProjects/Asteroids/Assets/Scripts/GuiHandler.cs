using UnityEngine;
using System.Collections;
using ModestTree.Zenject;

namespace Asteroids
{
    public class GuiHandler : MonoBehaviour
    {
        [Inject]
        public GameController _gameController;

        public GUIStyle titleStyle;
        public GUIStyle instructionsStyle;
        public GUIStyle gameOverStyle;
        public GUIStyle timeStyle;

        public float gameOverFadeInTime;
        public float gameOverStartFadeTime;

        public float restartTextStartFadeTime;
        public float restartTextFadeInTime;

        float _gameOverElapsed;

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            {
                switch (_gameController.State)
                {
                    case GameState.WaitingToStart:
                        StartGui();
                        break;

                    case GameState.Playing:
                        PlayingGui();
                        break;

                    case GameState.GameOver:
                        PlayingGui();
                        GameOverGui();
                        break;

                    default:
                        Util.Assert(false);
                        break;
                }
            }
            GUILayout.EndArea();
        }

        void GameOverGui()
        {
            _gameOverElapsed += Time.deltaTime;

            if (_gameOverElapsed > gameOverStartFadeTime)
            {
                var px = Mathf.Min(1.0f, (_gameOverElapsed - gameOverStartFadeTime) / gameOverFadeInTime);
                titleStyle.normal.textColor = new Color(1, 1, 1, px);
            }
            else
            {
                titleStyle.normal.textColor = new Color(1, 1, 1, 0);
            }

            if (_gameOverElapsed > restartTextStartFadeTime)
            {
                var px = Mathf.Min(1.0f, (_gameOverElapsed - restartTextStartFadeTime) / restartTextFadeInTime);
                instructionsStyle.normal.textColor = new Color(1, 1, 1, px);
            }
            else
            {
                instructionsStyle.normal.textColor = new Color(1, 1, 1, 0);
            }

            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical();
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginVertical();
                    {
                        GUILayout.FlexibleSpace();

                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.FlexibleSpace();
                            GUILayout.Label("GAME OVER", titleStyle);
                            GUILayout.FlexibleSpace();
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.Space(60);

                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.FlexibleSpace();

                            GUILayout.Label("Click to restart", instructionsStyle);

                            GUILayout.FlexibleSpace();
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();

                    GUILayout.FlexibleSpace();
                }

                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }

        void PlayingGui()
        {
            GUILayout.BeginVertical();
            {
                GUILayout.Space(30);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(30);
                    GUILayout.Label("Time: " + _gameController.ElapsedTime.ToString("0.##"), timeStyle);
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        void StartGui()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical();
                {
                    GUILayout.Space(100);
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginVertical();
                    {
                        GUILayout.FlexibleSpace();

                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.FlexibleSpace();
                            GUILayout.Label("ASTEROIDS", titleStyle);
                            GUILayout.FlexibleSpace();
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.Space(60);

                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.FlexibleSpace();

                            GUILayout.Label("Click to start", instructionsStyle);

                            GUILayout.FlexibleSpace();
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();

                    GUILayout.FlexibleSpace();
                }

                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }

        void Start()
        {
            GameEvent.ShipCrashed += OnShipCrashed;
        }

        void OnShipCrashed()
        {
            _gameOverElapsed = 0;
        }
    }
}

