using TMPro;
using UnityEngine;

namespace UI
{
    public class UIMainCanvas : SingletonPersistentMono<UIMainCanvas>
    {
        public TMP_Text scoreText;
        private int score = 0;

        public int Score
        {
            get{return score;}
            set
            {
                score = value;
                scoreText.text = score.ToString();
            }
        }
    }
}
