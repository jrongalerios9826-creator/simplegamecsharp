using Godot;

// Inheritance: Fighter gets Speed, Gravity, Health, Move(), TakeDamage()
// all for free from Character, and adds attacking on top
public partial class Fighter : Character
{
	[Export] public int PlayerNumber = 1;

	// Polymorphism: subclasses override these to use their own controls and animations
	protected virtual string InputPrefix => PlayerNumber == 1 ? "p1" : "p2";
	protected virtual string Attack1Action => InputPrefix + "_punch";
	protected virtual string Attack2Action => InputPrefix + "_punch2";
	protected virtual string Attack1Anim => "attack1";
	protected virtual string Attack2Anim => "attack2";

	private bool _isAttacking = false;
	private float _attackTimer = 0f;
	private const float AttackDuration = 0.25f;
	private float _hitFlashTimer = 0f;

	private Area2D _attackArea;

	public override void _Ready()
	{
		base._Ready();

		_attackArea = GetNode<Area2D>("AttackArea");
		_attackArea.Monitoring = false;
		_attackArea.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;
		Vector2 velocity = Velocity;

		if (!IsOnFloor())
			velocity.Y += Gravity * dt;

		string prefix = InputPrefix;

		float moveInput = 0f;
		if (Input.IsActionPressed(prefix + "_left")) moveInput -= 1;
		if (Input.IsActionPressed(prefix + "_right")) moveInput += 1;

		bool jumpPressed = Input.IsActionJustPressed(prefix + "_jump");
		bool punchPressed = Input.IsActionJustPressed(Attack1Action);
		bool punch2Pressed = Input.IsActionJustPressed(Attack2Action);

		if (!_isAttacking)
		{
			Move(moveInput, jumpPressed, ref velocity);

			if (punchPressed)
				StartAttack(Attack1Anim);
			else if (punch2Pressed)
				StartAttack(Attack2Anim);
		}
		else
		{
			velocity.X = 0;
		}

		Velocity = velocity;
		MoveAndSlide();

		if (_isAttacking)
		{
			_attackTimer -= dt;
			if (_attackTimer <= 0)
				EndAttack();
		}

		UpdateHitFlash(dt);
	}

	protected void UpdateHitFlash(float dt)
	{
		if (_hitFlashTimer > 0)
		{
			_hitFlashTimer -= dt;
			if (_hitFlashTimer <= 0 && !_isAttacking)
				_sprite.Modulate = Colors.White;
		}
	}

	private void StartAttack(string animationName)
	{
		_isAttacking = true;
		_attackTimer = AttackDuration;

		_attackArea.Monitoring = true;
		var shape = _attackArea.GetNode<CollisionShape2D>("CollisionShape2D");
		shape.Disabled = false;
		shape.Position = new Vector2(_facingRight ? 30 : -30, 0);

		_sprite.Play(animationName);
		_sprite.Modulate = Colors.Yellow;
	}

	private void EndAttack()
	{
		_isAttacking = false;
		_attackArea.Monitoring = false;
		_attackArea.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;

		_sprite.Modulate = Colors.White;
	}

	public void OnAttackAreaBodyEntered(Node2D body)
	{
		if (body is Fighter other && other != this && _isAttacking)
		{
			other.TakeDamage(10);
		}
	}

	public override void TakeDamage(int amount)
	{
		base.TakeDamage(amount);
		_sprite.Modulate = Colors.Red;
		_hitFlashTimer = 0.15f;
	}
}
