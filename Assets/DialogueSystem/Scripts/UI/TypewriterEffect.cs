using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace DS.UI
{
    public class TypewriterEffect : MonoBehaviour
    {

        [SerializeField] private float typewriterSpeed = 50f;

        public bool isRunning { get; private set; }

        private readonly List<Punctuation> punctuations = new List<Punctuation>()
        {
            new Punctuation(new HashSet<char>() { '.', '!', '?' }, 0.6f),
            new Punctuation(new HashSet<char>() { ',', ';', ':' }, 0.3f)
        };

        private Coroutine typingCoroutine;

        public void Run(string textToType, TMP_Text textLabel)
        {
            typingCoroutine = StartCoroutine(TypeText(textToType, textLabel));
        }

        public void Stop()
        {
            StopCoroutine(typingCoroutine);

            isRunning = false;
        }

        private IEnumerator TypeText(string textToType, TMP_Text textLabel)
        {
            isRunning = true;
            textLabel.text = string.Empty;

            float t = 0;
            int charIndex = 0;

            while (charIndex < textToType.Length)
            {
                int lastCharIndex = charIndex;

                t += Time.deltaTime * typewriterSpeed;
                charIndex = Mathf.FloorToInt(t);
                charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);

                for (int i = lastCharIndex; i < charIndex; i++)
                {
                    bool isLast = i >= textToType.Length - 1;

                    textLabel.text = textToType.Substring(0, i + 1);

                    if (IsPunctuation(textToType[i], out float waitTime) && !isLast &&
                        !IsPunctuation(textToType[i + 1], out _))
                    {
                        yield return new WaitForSeconds(waitTime);
                    }
                }

                yield return null;
            }

            isRunning = false;
        }

        private bool IsPunctuation(char character, out float waitTime)
        {
            foreach (Punctuation punctuationCategory in punctuations)
            {
                if (punctuationCategory.Punctuations.Contains(character))
                {
                    waitTime = punctuationCategory.WaitTime;
                    return true;
                }
            }

            waitTime = default;
            return false;
        }

        private readonly struct Punctuation
        {
            public readonly HashSet<char> Punctuations;
            public readonly float WaitTime;

            public Punctuation(HashSet<char> punctuations, float waitTime)
            {
                Punctuations = punctuations;
                WaitTime = waitTime;
            }
        }
    }
}
