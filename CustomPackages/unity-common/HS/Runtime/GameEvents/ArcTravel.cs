using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace HS
{
	public class ArcTravel : MonoBehaviour
	{
		public ArcTravelSettings Settings;


		[SerializeField] private Vector3 start, end;
		[SerializeField] public GameObject Traveller;
		private System.Action callback = null;


		private float _duration;



		public static ArcTravel Create(GameObject go, Vector3 end, System.Action callback = null, ArcTravelSettings settings = null ) =>
			Create(go, go.transform.position, end, callback, settings);

		public static ArcTravel Create(GameObject go, Vector3 start, Vector3 end, System.Action callback = null, ArcTravelSettings settings = null )
		{
			DefaultsManager.TriggerDefaulLoad();
			
			foreach (var arc in go.GetComponents<ArcTravel>())
				Destroy(arc);

			ArcTravel arcTravel = go.AddComponent<ArcTravel>();
			arcTravel.start = start;
			arcTravel.end = end;
			arcTravel.Traveller = go;
			arcTravel.callback = callback;
			arcTravel.Settings = settings ?? ArcTravelSettings.Default;
			return arcTravel;
		}
		IEnumerator Start()
		{
			yield return null;

			if( !Settings ) Settings = ArcTravelSettings.Default;

			float startTime = Time.time;
			float distance = (start - end).magnitude;
			_duration =
				Mathf.Min(
					distance / Settings.Speed,
					Settings.MaxTime > 0
						? Settings.MaxTime
						: Mathf.Infinity
				);

			if (Settings.StartEffectPrefab != null)
			{
				GameObject go = HS.Pool.Instance.GetSpawnFromPrefab(Settings.StartEffectPrefab);
				if (go) go.transform.position = start;
			}


			while (Time.time < (startTime + _duration))
			{
				float phase = (Time.time - startTime) / _duration;
				float evaluate = Settings.Curve.Evaluate(phase);

				Vector3 newPos = Vector3.Lerp(start, end, phase);
				newPos.y += evaluate * (distance * Settings.YFactor);
				Traveller.gameObject.transform.position = newPos;
				yield return null;
			}

			if (Traveller.GetComponent<FreeFlyCamera>()) Traveller.GetComponent<FreeFlyCamera>().enabled = true;

			if (Settings.EndEffectPrefab != null)
			{
				GameObject go = HS.Pool.Instance.GetSpawnFromPrefab(Settings.EndEffectPrefab);
				if (go) go.transform.position = end;
			}


			callback?.Invoke();
			Destroy(this);
		}


		//Debugging purposes
		[ContextMenu("StartOver")]
		public void StartAgain()
		{
			StartCoroutine(Start());
		}
	}
}