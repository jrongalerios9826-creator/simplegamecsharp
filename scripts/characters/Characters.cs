using Godot;

// Abstraction: Character defines the general concept of "a physical body
// with health and movement" without knowing anything about punching
public abstract partial class Character : CharacterBody2D
{
	[Export] public float Speed = 200f;
	[Export] public float JumpVelocity = -350f;
	[Export] public float Gravity = 900f;
	[Export] public int MaxHealth = 100;

	// Encapsulation: Health can be READ by anything, but only changed
	// from inside this class (via TakeDamage)
	public int Health { get; protected set; }

	protected bool _facingRight = true;
	protected AnimatedSprite2D _sprite;

	public override void _Ready()
	{
		Health = MaxHealth;
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_sprite.Play("idle");
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

	// Polymorphism: animation selection is its own overridable method —
	// a subclass could customize how it picks animations without touching Move()
	protected virtual void UpdateAnimation(float moveInput, Vector2 velocity)
	{
		if (!IsOnFloor())
		{
			if (velocity.Y < 0)
			{
				if (_sprite.Animation != "jump")
					_sprite.Play("jump");
			}
			else
			{
				if (_sprite.Animation != "fall")
					_sprite.Play("fall");
			}
		}
		else if (moveInput != 0)
		{
			if (_sprite.Animation != "run")
				_sprite.Play("run");
		}
		else
		{
			if (_sprite.Animation != "idle")
				_sprite.Play("idle");
		}
	}

	// Polymorphism: every Character can take damage, but HOW it reacts
	// can be overridden differently by subclasses
	public virtual void TakeDamage(int amount)
	{
		Health = Mathf.Max(Health - amount, 0);
		GD.Print(Name, " health: ", Health);
	}
}
