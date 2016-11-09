static var instance			: ActionsHUD;
instance = GetComponent(ActionsHUD);
	var	middle_label: Texture2D;
	var area				: Rect;
	var	ActiveGlyphs		: Texture2D[];
	var	InactiveGlyphs		: Texture2D[];
	var	State				: int = eAvailableActions.None;
	
	var guiSkin: GUISkin;
	function DrawGlyph(index : int, state: int,  x: int, y: int, area : Rect, tool_tip)
	{
			area.x = x;
			area.y = y;
			
			if ( GUI.Button(area, GUIContent(((State & state) == state) ? ActiveGlyphs[index] : InactiveGlyphs[index], tool_tip), "Label") )
				gameObject.BroadcastMessage( "PerformAction", state );
				
	}
	
	function OnGUI()
	{				
//		if (Globals.playMode != ePlaymode.Playing)
//			return;
		GUI.skin = guiSkin;	
		var glyphArea		= Rect(0,0,area.width * 0.36, area.height * 0.36);

		GUI.BeginGroup(area);
			DrawGlyph(0,eAvailableActions.Look		, 0					, area.height * 0.32	, glyphArea, "Look at");
			DrawGlyph(1,eAvailableActions.Interact	, area.width * 0.32	, 0						, glyphArea, "Touch/pick up");
			DrawGlyph(2,eAvailableActions.SpeakTo	, area.width * 0.64	, area.height * 0.32	, glyphArea, "Talk to");
			//DrawGlyph(3,eAvailableActions.Collect	, area.width * 0.32	, area.height * 0.64	, glyphArea, "Collect");
		GUI.EndGroup();
		GUI.Label (Rect (50,50,100, 100), middle_label);
		GUI.Label (Rect (49,52,100, 100), GUI.tooltip);
	}
	
	
	function SetState(to : int)
	{
		State = to;
	}
	
