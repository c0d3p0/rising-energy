using System.Text;
using SCG = System.Collections.Generic;

using Godot;
using Godot.Collections;


public class PlayerInputInterpreter : Node
{
	private bool InterpretInteract()
	{
		return IsActionInputPressed(ATTACK_MASK) &&
		 		this.EmitSignal<bool>(this, this.GetSignalInteract());
	}

	private bool InterpretCancelInteraction()
	{
		return IsActionInputPressed(JUMP_MASK) &&
		 		this.EmitSignal<bool>(this, this.GetSignalCancelInteraction());
	}

	// private bool InterpretBlock()
	// {
	// 	return Input.IsActionPressed(inputBlock) &&
	// 	 		this.EmitSignal<bool>(this, this.GetSignalBlock(), direction);
	// }

	private bool InterpretAttack()
	{
		return IsActionInputPressed(ATTACK_MASK) &&
		 		this.EmitSignal<bool>(this, this.GetSignalAttack(), direction);
	}

	private bool InterpretJump()
	{
		return IsActionInputPressed(JUMP_MASK) &&
		 		this.EmitSignal<bool>(this, this.GetSignalJump(), direction);
	}

	private bool InterpretCrouch()
	{
		return direction.y == -1 &&
				this.EmitSignal<bool>(this, this.GetSignalCrouch(), direction);
	}

	private bool InterpretMove()
	{
		return  this.EmitSignal<bool>(this, this.GetSignalMove(), direction);
	}

	private bool InterpretAction(string signal, bool ignore)
	{
		if(!ignore)
			return this.EmitSignal<bool>(this, signal);

		return ignore;
	}

	private bool InterpretAction(string signal, Vector3 direction, bool ignore)
	{
		if(!ignore)
			return this.EmitSignal<bool>(this, signal, direction);

		return ignore;
	}

	private void ComputeDirectionAxisInput(ref float axis, string negative, string positive)
	{
		bool axisNegative = Input.IsActionPressed(negative);
		bool axisPositve = Input.IsActionPressed(positive);

		if(axisPositve && !axisNegative)
			axis = 1;
		else if(axisNegative && !axisPositve)
			axis = -1;
		else
			axis = 0;
	}

	private void ComputerActionInputs()
	{
		byte actionInputMask = (byte) (Input.IsActionPressed(inputAttack) ? ATTACK_MASK : 0);
		actionInputMask |= (byte) (Input.IsActionPressed(inputJump) ? JUMP_MASK : 0);
		actionInputMask |= (byte) (Input.IsActionPressed(inputBlock) ? BLOCK_MASK : 0);
		currentActionInputMask = actionInputMask;
		
		while(actionInputBufferList.Count >= actionInputBufferLength)
			actionInputBufferList.RemoveAt(0);
		
		actionInputBufferList.Add(actionInputMask);
		ComputeActionInputUnionMask();
	}

	private void ComputeActionInputUnionMask()
	{
		SCG.IEnumerator<byte> it = actionInputBufferList.GetEnumerator();
		actionInputUnionMask = ATTACK_MASK | JUMP_MASK | BLOCK_MASK;

		while(it.MoveNext())
			actionInputUnionMask &= it.Current;
	}

	private bool IsActionInputPressed(byte actionInput)
	{
		return ((actionInput & actionInputUnionMask) == 0) &&
				((actionInput & currentActionInputMask) == actionInput);
	}

	private void HandleDirectionInput()
	{
		direction.x = 0;
		direction.y = 0;
		ComputeDirectionAxisInput(ref direction.x, inputLeft, inputRight);
		ComputeDirectionAxisInput(ref direction.y, inputDown, inputUp);
	}

	private string GetFixedInputName(string inputName)
	{
		return new StringBuilder().Append('p').Append(playerIndex + 1).
				Append('_').Append(inputName).ToString();
	}

	private void InterpretInputs()
	{
		HandleDirectionInput();
		ComputerActionInputs();

		bool ignore = InterpretInteract();

		if(!ignore)
			ignore = InterpretJump();

		if(!ignore)
			ignore = InterpretCancelInteraction();
		
		// if(!ignore)
		// 	ignore = InterpretBlock();

		if(!ignore)
			ignore = InterpretAttack();

		if(!ignore)
			ignore = InterpretCrouch();

		if(!ignore)
			ignore = InterpretMove();
	}

	private void Initialize()
	{
		actionInputBufferList = new Array<byte>();
		actionInputBufferList.Add(0);
		Input.SetMouseMode(Input.MouseMode.Captured);
		inputUp = GetFixedInputName(inputUp);
		inputDown = GetFixedInputName(inputDown);
		inputLeft = GetFixedInputName(inputLeft);
		inputRight = GetFixedInputName(inputRight);
		inputAttack = GetFixedInputName(inputAttack);
		inputJump = GetFixedInputName(inputJump);
		inputBlock = GetFixedInputName(inputBlock);
	}

	public override void _EnterTree()
	{
		Initialize();
	}

	public override void _PhysicsProcess(float delta)
	{
		InterpretInputs();
	}

	public byte PlayerIndex
	{
		set
		{
			playerIndex = value;
		}
	}


	[Export]
	private byte actionInputBufferLength = 5;

	private byte playerIndex;

	private Vector3 direction;
	private Array<byte> actionInputBufferList;
	private byte currentActionInputMask;
	private byte actionInputUnionMask;

	private string inputUp = "up";
	private string inputDown = "down";
	private string inputLeft = "left";
	private string inputRight = "right";
	private string inputAttack = "attack";
	private string inputJump = "jump";
	private string inputBlock = "block";

	private const byte ATTACK_MASK = 1;
	private const byte JUMP_MASK = 2;
	private const byte BLOCK_MASK = 4;
}
