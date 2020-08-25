using Godot;
using Godot.Collections;


public class PlayerCharacterAction : BaseCharacterAction
{
	public void Hit(Area attackerArea, Area victimArea, uint strikeId,
			float damageTaken, float energyGiven)
	{
		if(!cheer && !ignoreHit)
		{
			bool hitLeft = attackerArea.GlobalTransform.origin.x <
					body.GlobalTransform.origin.x;
			ignoreHit = true;
			FixBodyDirection(hitLeft ? Vector3.Left : Vector3.Right);
			EmitSignal(this.GetSignalPrepareHurtHop(), hitLeft ? Vector3.Right : Vector3.Left);
			EmitSignal(this.GetSignalHealthChanged(), damageTaken);
			EmitSignal(this.GetSignalEnergyChanged(), energyGiven);
			playerCharacter.Call(this.GetMethodActivateInput(), false);
			animationStateMachine.Travel("trigger_hit");
		}
	}

	public bool Interact()
	{
		string a = CanTransitToInteract();

		if(!cheer && a != null)
		{
			if(interactionObject != null && !interactionObject.IsQueuedForDeletion())
			{	
				if(interactionObject.IsInGroup("energy_source"))
					return BoostEnergy(a);

				if(interactionObject.IsInGroup("warrior_spirit"))
					return TalkToWarriorSpirit();
			}
			else
				return CancelInteraction();
		}

		return false;
	}

	public bool CancelInteraction()
	{
		string a = CanTransitToCancelInteraction();

		if(!cheer && a != null)
			return CancelBoostEnergy();

		return false;
	}

	// public bool Block(Vector3 direction)
	// {
	// 	string t = CanTransitToBlock();

	// 	if(t != null && this.EmitSignal<bool>(this, this.GetSignalCanBlock()))
	// 	{
	// 		EmitSignal(this.GetSignalAttack(), blockAction);
	// 		FixDirection(direction);
	// 		return true;
	// 	}

	// 	return false;
	// }

	public bool Attack(Vector3 direction)
	{
		string a = CanTransitToAttack();

		if(!cheer && a != null && this.EmitSignal<bool>(this, this.GetSignalCanAttack()))
		{
			FixBodyDirection(direction);
			animationStateMachine.Travel(a);
			playerCharacter.Call(this.GetMethodActivateInput(), false);
			return true;
		}

		return false;
	}

	public bool Jump(Vector3 direction)
	{
		string a = CanTransitToJump();

		if(!cheer && a != null)
		{
			FixBodyDirection(direction);
			EmitSignal(this.GetSignalPrepareJump(), direction);
			animationStateMachine.Travel(a);
			playerCharacter.Call(this.GetMethodActivateInput(), false);
			return true;
		}

		return false;
	}

	public bool Crouch(Vector3 direction)
	{
		string a = CanTransitToCrouch();

		if(!cheer && a != null)
		{
			FixBodyDirection(direction);
			animationStateMachine.Travel(a);
			return true;
		}

		return false;
	}

	public bool Move(Vector3 direction)
	{
		string a = CanTransitToMove();

		if(!cheer && onGround && a != null)
		{
			if(direction.x != 0 && a.Equals("run"))
			{
				FixBodyDirection(direction);
				EmitSignal(this.GetSignalApplyMove(), direction);
				animationStateMachine.Travel(a);
			}
			else
			{
				EmitSignal(this.GetSignalApplyMove(), Vector3.Zero);
				animationStateMachine.Travel("default_idle");
			}
		}

		return false;
	}				

	public void SetToDialogueInteraction(bool active)
	{
		ignoreHit = active;
		animationStateMachine.Travel(active ? "pre_immortal_idle" : "default_idle");
	}

	private bool BoostEnergy(string action)
	{
		bool interaction = this.Call<bool>(interactionObject, this.GetMethodInteract(),
				playerCharacter, action.Equals("boost_energy") ? 1 : 0);
				
		if(interaction)
			animationStateMachine.Travel("boost_energy");
		else if(action.Equals("boost_energy") || action.Equals("boost_energy_to_stand"))
			animationStateMachine.Travel("default_idle");

		return interaction;
	}

	private bool CancelBoostEnergy()
	{
		animationStateMachine.Travel("default_idle");

		if(interactionObject != null)
		{
			interactionObject.Call(this.GetMethodInteract(), playerCharacter);
			this.Call<bool>(interactionObject,
					this.GetSignalInteract(), playerCharacter, -1);
		}
		
		return true;
	}

	private bool TalkToWarriorSpirit()
	{
		playerCharacter.Call(this.GetMethodSetToDialogueInteraction(), true);
		interactionObject.Call(this.GetMethodInteract(), playerCharacter);
		return true;
	}

	private bool Fall()
	{
		if(!onGround && CanTransitToFalling())
		{
			if(this.EmitSignal<bool>(this, this.GetSignalDead()))
				animationStateMachine.Travel("die_falling");
			else
				animationStateMachine.Travel("falling");

			return true;
		}

		return false;
	}

	private bool LandOnTheGround()
	{
		if(onGround && toLandingMap.ContainsKey(
				animationStateMachine.GetCurrentNode()))
		{
			if(this.EmitSignal<bool>(this, this.GetSignalDead()))
				animationStateMachine.Travel("die");
			else
				animationStateMachine.Travel("default_idle");

			return true;
		}

		return false;
	}

	private bool LandHurtOnTheGround()
	{
		if(onGround && animationStateMachine.GetCurrentNode().Equals("hit_falling"))
		{
			if(this.EmitSignal<bool>(this, this.GetSignalDead()))
				animationStateMachine.Travel("die");
			else
				animationStateMachine.Travel("get_up");

			return true;
		}

		return false;
	}

	public void Cheer()
	{
		cheer = true;
		playerCharacter.Call(this.GetMethodActivateInput(), false);
	}	

	public void Die() // Called by animation
	{
		playerCharacter.EmitSignal(this.GetSignalDead());
	}

	private void ForceCheer()
	{
		if(cheer)
		{
			playerCharacter.Call(this.GetMethodActivateInput(), false);
			string a = CanTransitToCheer();

			if(a != null)
				animationStateMachine.Travel("cheer");
		}
	}

	private void UpdateOnGround()
	{
		onGround = this.EmitSignal<bool>(this, this.GetSignalIsOnGround());
	}

	public void OnCameraExited(Camera camera)
	{
		animationStateMachine.Travel("die_falling");

	}

	protected string CanTransitToInteract()
	{	
		return GetValue(toInteractMap, animationStateMachine.GetCurrentNode());
	}

	protected string CanTransitToCancelInteraction()
	{	
		return GetValue(toCancelInteractMap, animationStateMachine.GetCurrentNode());
	}

	protected string CanTransitToCheer()
	{	
		return GetValue(toCheerMap, animationStateMachine.GetCurrentNode());
	}

	protected bool CanTransitToFalling()
	{	
		return !toFallingExcludeMap.ContainsKey(
				animationStateMachine.GetCurrentNode());
	}

	protected override void Initialize()
	{
		base.Initialize();
		playerCharacter = GetNode<KinematicBody>(playerCharacterNP);
	}

	public override void _Process(float delta)
	{
		UpdateOnGround();
		LandOnTheGround();
		LandHurtOnTheGround();
		Fall();
		ForceCheer();
	}

	public Spatial InteractionObject
	{
		set
		{
			interactionObject = value;
		}	
	}


	[Export]
	public NodePath playerCharacterNP;

	[Export]
	public Dictionary<string, string> toInteractMap;

	[Export]
	public Dictionary<string, string> toCancelInteractMap;

	[Export]
	public Dictionary<string, string> toFallingExcludeMap;

	[Export]
	public Dictionary<string, string> toLandingMap;

	[Export]
	public Dictionary<string, string> toCheerMap;

	[Export]
	public bool ignoreHit;


	private KinematicBody playerCharacter;

	private Spatial interactionObject;
	private bool onGround;
	private bool cheer;
}
