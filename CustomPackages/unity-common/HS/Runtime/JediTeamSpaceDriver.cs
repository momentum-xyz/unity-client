using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
	public class JediTeamSpaceDriver : MonoBehaviour
	{
		public void SetupTeamPartner( Partner partner )
		{
			if( _partnerGraphics.ContainsKey(partner) )
				_driver.NewTeamPoster( _partnerGraphics[partner] );
		}


		[SerializeField] List<PartnerGraphicsCouple> PartnerGraphics;
		Dictionary<Partner,Texture2D> _partnerGraphics = new Dictionary<Partner,Texture2D>();

		TeamSpaceDriver _driver;

		[System.Serializable]
		public struct PartnerGraphicsCouple{
			public Partner Partner;
			public Texture2D Texture;
		}

		void Awake()
		{
			_driver = GetComponent<TeamSpaceDriver>();
			_partnerGraphics.Clear();
			foreach( var elm in PartnerGraphics ) _partnerGraphics.Add( elm.Partner, elm.Texture );
		}


		public enum Partner{
			NEN,
			BOIP,
			Deloitte,
			IOTA,
			Waves,
			LoyensAndLoef,
			SoftwareImprovementGroup
		}
	}
}