using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace FishEatFishSpace
{
	public class DebugPage : MonoBehaviour
	{
		[SerializeField] GameObject cheatGroup;
		[SerializeField] GameObject infoGroup;

		[SerializeField] GameObject infoPop;

		// [SerializeField] GameObject infoLoading;
		[SerializeField] Text logTxt;

		bool isDebug = false;
		float deltaTime = 0.0f;

		void Awake()
		{
			infoGroup.SetActive(false);
		}

		void Update()
		{
			if (isDebug)
			{
				deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
			}
		}

		void OnGUI()
		{
			if (isDebug)
			{
				int w = Screen.width, h = Screen.height;

				GUIStyle style = new GUIStyle();

				Rect rect = new Rect(50f, h * 0.98f - 50f, w - 100f, h - 50f);
				style.alignment = TextAnchor.UpperRight;
				style.fontSize = h * 2 / 100;
				style.normal.textColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
				// float msec = deltaTime * 1000.0f;
				float fps = 1.0f / deltaTime;
				string text = string.Format("    fps: {0:0.}", fps);
				GUI.Label(rect, text, style);
			}
		}

		public void Init(bool open_cheat)
		{
			isDebug = open_cheat;
			cheatGroup.SetActive(open_cheat);
		}

		// public void ClickPreview()
		// {
		//     if(Profile.Instance.Level>1)
		//     {
		//         Profile.Instance.Level--;
		//     }
		//     SceneManager.LoadScene("Demo",LoadSceneMode.Single);
		// }

		// public void ClickNext()
		// {
		//     Profile.Instance.Level++;
		//     SceneManager.LoadScene("Demo",LoadSceneMode.Single);
		// }

		public void ClickCash()
		{
			//Profile.Instance.Cash += 5000;
		}

		void ShowPop(bool show_info)
		{
			infoPop.SetActive(show_info);
			// infoLoading.SetActive(!show_info);
			infoGroup.SetActive(true);
		}

		public void ShowErrorInfo(string log, string trace)
		{
			logTxt.text = $"{log}\n{trace}";
			ShowPop(true);
		}

		public void RestartGame()
		{
			//SceneManager.LoadScene("Main",LoadSceneMode.Single);
			infoGroup.SetActive(false);
		}

		public void ShowErrorRestart()
		{

		}
	}
}