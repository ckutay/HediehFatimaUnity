﻿using System;
using ActionLibrary;
using Conditions;
using GAIPS.Serialization;
using SocialImportance.DTOs;
using WellFormedNames;

namespace SocialImportance
{
	[Serializable]
	internal class Conferral : BaseActionDefinition
	{
		public uint ConferralSI { get; set; }

		public Conferral(uint conferralSI, Name actionTemplate, Name target, ConditionSet activationConditions) : base(actionTemplate, target, activationConditions)
		{
			ConferralSI = conferralSI;
		}

		public Conferral(Conferral other) : base(other)
		{
			ConferralSI = other.ConferralSI;
		}

		public Conferral(ConferralDTO dto) : base(dto)
		{
			ConferralSI = dto.ConferralSI;
		}

		public ConferralDTO ToDTO()
		{
			return FillDTO(new ConferralDTO() {ConferralSI = ConferralSI});
		}

		public void SetData(ConferralDTO dto)
		{
			ConferralSI = dto.ConferralSI;
			SetFromDTO(dto);
		}

		protected override float CalculateActionUtility(IAction a)
		{
			return float.PositiveInfinity;
		}

		public override void GetObjectData(ISerializationData dataHolder, ISerializationContext context)
		{
			dataHolder.SetValue("ConferralSI",ConferralSI);
			base.GetObjectData(dataHolder, context);
		}

		public override void SetObjectData(ISerializationData dataHolder, ISerializationContext context)
		{
			base.SetObjectData(dataHolder, context);
			ConferralSI = dataHolder.GetValue<uint>("ConferralSI");
		}
	}
}