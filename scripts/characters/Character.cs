using Godot;

// Abstraction: Character defines the general concept of "a physical body
// with health and movement" without knowing anything about punching
public abstract partial class Character : CharacterBody2D
{
	[Export] public float Speed = 200f;
	[Export] public float JumpVelocity = -350f;
	[Export] public float Gravity = 900f;
	[Export] public int MaxHealth = 100;
	[Export] public bool StartFacingRight = true;

	// Encapsulation: Health can be READ by anything, but only changed
	// from inside this class (via TakeDamage)
	public int Health { get; protected set; }

	protected bool _facingRight = true;
	protected AnimatedSprite2D _sprite;

	// Polymorphism: subclasses can override these to use their own animation names
	protected virtual string IdleAnim => "idle";
	protected virtual string RunAnim => "run";
	protected virtual string JumpAnim => "jump";
	protected virtual string FallAnim => "fall";

	public override void _Ready()
	{
		// Make sure floor detection works no matter what the Inspector says
		MotionMode = MotionModeEnum.Grounded;
		UpDirection = Vector2.Up;

		Health = MaxHealth;
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_facingRight = StartFacingRight;
		_sprite.FlipH = !_facingRight;
		_sprite.Play(IdleAnim);
	}

	protected void Move(float moveInput, bool jumpPressed, ref Vector2 velocity)
	{
		velocity.X = moveInput * Speed;

		if (moveInput > 0) _facingRight = true;
		else if (moveInput < 0) _facingRight = false;
		_sprite.FlipH = !_facingRight;

		if (jumpPressed && IsOnFloor())
			velocity.Y = JumpVelocity;

		UpdateAnimation(moveInput, velocity);
	}

	protected virtual void UpdateAnimation(float moveInput, Vector2 velocity)
	{
		if (!IsOnFloor())
		{
			if (velocity.Y < 0)
			{
				if (_sprite.Animation != JumpAnim)
					_sprite.Play(JumpAnim);
			}
			else
			{
				if (_sprite.Animation != FallAnim)
					_sprite.Play(FallAnim);
			}
		}
		else if (moveInput != 0)
		{
			if (_sprite.Animation != RunAnim)
				_sprite.Play(RunAnim);
		}
		else
		{
			if (_sprite.Animation != IdleAnim)
				_sprite.Play(IdleAnim);
		}
	}

	public virtual void TakeDamage(int amount)
	{
		Health = Mathf.Max(Health - amount, 0);
		GD.Print(Name, " health: ", Health);
	}
}
