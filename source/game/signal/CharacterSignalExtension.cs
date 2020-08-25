public static class CharacterPhysicsSignalExtension
{
	public static string GetSignalInteract(this Godot.Object gdObj)
	{
		return INTERACT;
	}

	public static string GetSignalCancelInteraction(this Godot.Object gdObj)
	{
		return CANCEL_INTERACTION;
	}

	public static string GetSignalBlock(this Godot.Object gdObj)
	{
		return BLOCK;
	}

	public static string GetSignalAttack(this Godot.Object gdObj)
	{
		return ATTACK;
	}

	public static string GetSignalJump(this Godot.Object gdObj)
	{
		return JUMP;
	}

	public static string GetSignalCrouch(this Godot.Object gdObj)
	{
		return CROUCH;
	}

	public static string GetSignalIdle(this Godot.Object gdObj)
	{
		return IDLE;
	}

	public static string GetSignalMove(this Godot.Object gdObj)
	{
		return MOVE;
	}

	public static string GetSignalHit(this Godot.Object gdObj)
	{
		return HIT;
	}

	public static string GetSignalSetInteractionObject(this Godot.Object gdObj)
	{
		return SET_INTERACTION_OBJECT;
	}

	public static string GetSignalDead(this Godot.Object gdObj)
	{
		return DEAD;
	}

	public static string GetSignalCheer(this Godot.Object gdObj)
	{
		return CHEER;
	}

	public static string GetSignalCanAttack(this Godot.Object gdObj)
	{
		return CAN_ATTACK;
	}

	public static string GetSignalCanBlock(this Godot.Object gdObj)
	{
		return CAN_BLOCK;
	}

	public static string GetSignalHealthChanged(this Godot.Object gdObj)
	{
		return HEALTH_CHANGED;
	}

	public static string GetSignalEnergyChanged(this Godot.Object gdObj)
	{
		return ENERGY_CHANGED;
	}

	public static string GetSignalApplyMove(this Godot.Object gdObj)
	{
		return APPLY_MOVE;
	}

	public static string GetSignalPrepareJump(this Godot.Object gdObj)
	{
		return PREPARE_JUMP;
	}

	public static string GetSignalPrepareHurtHop(this Godot.Object gdObj)
	{
		return PREPARE_HURT_HOP;
	}

	public static string GetSignalIsOnGround(this Godot.Object gdObj)
	{
		return IS_ON_GROUND;
	}

	public static string GetSignalIncreaseVelocity(this Godot.Object gdObj)
	{
		return INCREASE_VELOCITY;
	}

	public static string GetSignalIncreaseTranslation(this Godot.Object gdObj)
	{
		return INCREASE_TRANSLATION;
	}


	private const string INTERACT = "int";
	private const string CANCEL_INTERACTION = "c_i";
	private const string BLOCK = "blk";
	private const string ATTACK = "atk";
	private const string JUMP = "jmp";
	private const string CROUCH = "crc";
	private const string MOVE = "mov";
	private const string IDLE = "idl";
	private const string HIT = "hit";
	private const string DEAD = "ded";
	private const string CHEER = "cheer";

	private const string SET_INTERACTION_OBJECT = "s_i_o";

	private const string CAN_ATTACK = "c_a";
	private const string CAN_BLOCK = "c_b";

	private const string HEALTH_CHANGED = "h_c";
	private const string ENERGY_CHANGED = "e_c";

	private const string APPLY_MOVE = "a_m";
	private const string PREPARE_JUMP = "p_j";
	private const string PREPARE_HURT_HOP = "p_h_h";
	private const string IS_ON_GROUND = "i_o_g";
	private const string INCREASE_VELOCITY = "i_v";
	private const string INCREASE_TRANSLATION = "i_t";
}
