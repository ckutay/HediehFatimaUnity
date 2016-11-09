var	actionHUD				: ActionsHUD;

private var NearItems		: Collider[];
private var ItemsInView 	= new Array();
private var LastCount		: int = 0;
private var	LookingAt		: int = 0;
private var CurrentItem		: crActions = null;
var interactDistance		: float = 1.5;
var LookAngle				: float = 30;
private var talked = false;

function Start()
{
	actionHUD = GetComponent(ActionsHUD);
}

function CalculateActionState()
{
	var state : int;
	if (CurrentItem)
		 state = CurrentItem.UpdateState();
	else state = 0;
	actionHUD.SetState(state);
}

function TestItemsInView()
{
	var targetDir		: Vector3;
	var forward			: Vector3;
	var angle			: float;

	for (var x : Collider in ItemsInView)
	{
		var test1 : Vector3 = new Vector3(x.transform.position.x, 0, x.transform.position.z);
		var test2 : Vector3 = new Vector3(transform.position.x,   0, transform.position.z);
    	targetDir = test1 - test2;
   		forward = transform.forward;
   		angle = Vector3.Angle(targetDir, forward);
   
  		if (angle <= LookAngle || angle >= -LookAngle)
		{
			LastCount = -1;
			return;
   		}
	}
}

function  UpdateActionHUD()
{
	var targetDir		: Vector3;
	var forward			: Vector3;
	var angle			: float;
	var crA : crActions;
	NearItems = Physics.OverlapSphere(transform.position, interactDistance);
	if (NearItems.length != LastCount)
	{
		ItemsInView.Clear();
			
		for (var x : Collider in NearItems)
		{
			var test1 : Vector3 = new Vector3(x.transform.position.x, 0, x.transform.position.z);
			var test2 : Vector3 = new Vector3(transform.position.x,   0, transform.position.z);
	    	targetDir = test1 - test2;
    		forward = transform.forward;
	   		angle = Vector3.Angle(targetDir, forward);
   
    		if (angle <= LookAngle && angle >= -LookAngle)
 			{
   				crA = x.transform.gameObject.GetComponent(crActions);
    			if (crA != null)
	    			ItemsInView.Push(x);
    		}
		}

		if (!LookingAtSameObject() )
			LookAtClosestItem();
    }
}

function LookingAtSameObject() : boolean
{
	var bla:  crActions;
	if (CurrentItem)
		for (var x : Collider in ItemsInView)
		{
			bla = x.transform.gameObject.GetComponent(crActions);
			if (bla == CurrentItem)
			{
				return true;
			}
		}			
	return false;
}

function LookAtClosestItem()
{
	var Distance	: float = Mathf.Infinity;
	var sqrLen		: float;
	var index		: int = 0;
	var lookat		: int = 0;

	if (ItemsInView.length == 0)
	{
		LookAtItem(-1);
	} else
	{
		for (var x : Collider in ItemsInView)
		{
			sqrLen = (x.transform.position - transform.position).sqrMagnitude;
        	if( sqrLen < Distance )
        	{
				lookat		= index;
				Distance	= sqrLen;
        	}
        	index++;
		}
		LookAtItem(lookat);
	}
}

function LookAtItem(lookat : int)
{
	var cur = CurrentItem;
	if (cur)
		cur.HasFocus(false);
		
	LookingAt = lookat;
	
	if (lookat < 0)
		CurrentItem = null;
	else
	{
		var looking_at : Collider = ItemsInView[LookingAt];
		CurrentItem = looking_at.transform.gameObject.GetComponent(crActions);
		CurrentItem.HasFocus(true);
	}
	CalculateActionState();

}

function Update()
{
    //UpdateActionHUD();
//    switch (Globals.playMode)
//    {
//    	case ePlaymode.Playing:
	//if(talked == false)
	///{
		GetTalking(); //if you're close enough to a character, make them talk to you
		//talked = true;
		//}
 		   TestForKeyPress();
 		  if(CurrentItem)
 		  	CurrentItem.closeEnough = true;
// 		   break;
//     }
}

function GetTalking()
{
	 if (actionHUD.State == 0 || !CurrentItem)
    	return;
	if(CurrentItem.talked == false)
	{
		CurrentItem.PerformAction(4);
		CurrentItem.talked = true;
	}
		
	}

function PerformAction(which: int)
{
	if (CurrentItem)
		CurrentItem.PerformAction(which);
}

function TestForKeyPress()
{
    if (actionHUD.State == 0 || !CurrentItem)
    	return;
    	
	if ( Input.GetKeyDown("j") )
		CurrentItem.PerformAction(1);

	if ( Input.GetKeyDown("i") )
		CurrentItem.PerformAction(2);

	if ( Input.GetKeyDown("l") )
		CurrentItem.PerformAction(4);

	if ( Input.GetKeyDown("k") )
		CurrentItem.PerformAction(8);
}


