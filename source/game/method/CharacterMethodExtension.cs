public static class CharacterMethodExtension
{
	public static string GetMethodInteract(this Godot.Object gdObj)
	{
		return INTERACT;
	}

	public static string GetMethodCancelInteraction(this Godot.Object gdObj)
	{
		return CANCEL_INTERACTION;
	}

	public static string GetMethodBlock(this Godot.Object gdObj)
	{
		return BLOCK;
	}

	public static string GetMethodAttack(this Godot.Object gdObj)
	{
		return ATTACK;
	}

	public static string GetMethodJump(this Godot.Object gdObj)
	{
		return JUMP;
	}

	public static string GetMethodCrouch(this Godot.Object gdObj)
	{
		return CROUCH;
	}

	public static string GetMethodMove(this Godot.Object gdObj)
	{
		return MOVE;
	}

	public static string GetMethodHit(this Godot.Object gdObj)
	{
		return HIT;
	}

	public static string GetMethodCheer(this Godot.Object gdObj)
	{
		return CHEER;
	}

	public static string GetMethodSetInteractionObject(this Godot.Object gdObj)
	{
		return SET_INTERACTION_OBJECT;
	}

	public static string GetMethodFixBodyDirection(this Godot.Object gdObj)
	{
		return FIX_BODY_DIRECTION;
	}

	public static string GetMethodIsDead(this Godot.Object gdObj)
	{
		return IS_DEAD;
	}

	public static string GetMethodSetDirection(this Godot.Object gdObj)
	{
		return SET_DIRECTION;
	}	

	public static string GetMethodCanAttack(this Godot.Object gdObj)
	{
		return CAN_ATTACK;
	}

	public static string GetMethodCanBlock(this Godot.Object gdObj)
	{
		return CAN_BLOCK;
	}

	public static string GetMethodTransitTo(this Godot.Object gdObj)
	{
		return TRANSIT_TO;
	}

	public static string GetMethodDecreaseHealth(this Godot.Object gdObj)
	{
		return DECREASE_HEALTH;
	}

	public static string GetMethodDecreaseEnergy(this Godot.Object gdObj)
	{
		return DECREASE_ENERGY;
	}

	public static string GetMethodSetItems(this Godot.Object gdObj)
	{
		return SET_ITEMS;
	}

	public static string GetMethodApplyMove(this Godot.Object gdObj)
	{
		return APPLY_MOVE;
	}

	public static string GetMethodPrepareJump(this Godot.Object gdObj)
	{
		return PREPARE_JUMP;
	}

	public static string GetMethodIsOnGround(this Godot.Object gdObj)
	{
		return IS_ON_GROUND;
	}

	public static string GetMethodIncreaseVelocity(this Godot.Object gdObj)
	{
		return INCREASE_VELOCITY;
	}

	public static string GetMethodIncreaseTranslation(this Godot.Object gdObj)
	{
		return INCREASE_TRANSLATION;
	}	

	public static string GetMethodSetTarget(this Godot.Object gdObj)
	{
		return SET_TARGET;
	}		

	public static string GetMethodSetVitality(this Godot.Object gdObj)
	{
		return SET_VITALITY;
	}

	public static string GetMethodGetVitalityMap(this Godot.Object gdObj)
	{
		return GET_VITALITY_MAP;
	}

	public static string GetMethodActivateInput(this Godot.Object gdObj)
	{
		return ACTIVATE_INPUT;
	}

	public static string GetMethodSetToDialogueInteraction(this Godot.Object gdObj)
	{
		return SET_TO_DIALOGUE_INTERACTION;
	}

	public static string GetMethodSetDespawnTimeTolerance(this Godot.Object gdObj)
	{
		return SET_DESPAWN_TIME_TOLERANCE;
	}

	public static string GetMethodSetKinSpawnPivot(this Godot.Object gdObj)
	{
		return SET_KIN_SPAWN_PIVOT;
	}

	public static string GetMethodSetRepositionPivot(this Godot.Object gdObj)
	{
		return SET_REPOSITION_PIVOT;
	}

	public static string GetMethodSetNodeFactory(this Godot.Object gdObj)
	{
		return SET_NODE_FACTORY;
	}

	public static string GetMethodSetProjectileSpawnData(this Godot.Object gdObj)
	{
		return SET_PROJECTILE_SPAWN_DATA;
	}

	public static string GetMethodSetKinSpawnData(this Godot.Object gdObj)
	{
		return SET_KIN_SPAWN_DATA;
	}


	private const string INTERACT = "Interact";
	private const string CANCEL_INTERACTION = "CancelInteraction";
	private const string BLOCK = "Block";
	private const string ATTACK = "Attack";
	private const string JUMP = "Jump";
	private const string CROUCH = "Crouch";
	private const string MOVE = "Move";
	private const string HIT = "Hit";
	private const string CHEER = "Cheer";

	private const string SET_INTERACTION_OBJECT = "SetInteractionObject";
	private const string FIX_BODY_DIRECTION = "FixBodyDirection";
	
	private const string SET_DIRECTION = "SetDirection";

	private const string IS_DEAD = "IsDead";
	private const string CAN_ATTACK = "CanAttack";
	private const string CAN_BLOCK = "CanBlock";
	private const string TRANSIT_TO = "TransitTo";
	
	private const string DECREASE_HEALTH = "DecreaseHealth";
	private const string DECREASE_ENERGY = "DecreaseEnergy";
	private const string SET_ITEMS = "SetItems";

	private const string APPLY_MOVE = "ApplyMove";
	private const string PREPARE_JUMP = "PrepareJump";
	private const string IS_ON_GROUND = "IsOnGround";
	private const string INCREASE_VELOCITY = "IncreaseVelocity";
	private const string INCREASE_TRANSLATION = "IncreaseTranslation";

	private const string SET_TARGET = "SetTarget";
	private const string SET_VITALITY = "SetVitality";
	private const string GET_VITALITY_MAP = "GetVitalityMap";

	private const string ACTIVATE_INPUT = "ActivateInput";
	private const string SET_TO_DIALOGUE_INTERACTION = "SetToDialogueInteraction";
	
	private const string SET_DESPAWN_TIME_TOLERANCE = "SetDespawnTimeTolerance";
	private const string SET_KIN_SPAWN_PIVOT = "SetKinSpawnPivot";
	private const string SET_REPOSITION_PIVOT = "SetRepositionPivot";
	private const string SET_NODE_FACTORY = "SetNodeFactory";

	private const string SET_PROJECTILE_SPAWN_DATA = "SetProjectileSpawnData";
	private const string SET_KIN_SPAWN_DATA = "SetKinSpawnData";
}

