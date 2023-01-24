using System;
using Android.OS;
using Merseyrail.Events;

namespace Merseyrail.Timers;

public class RedeemTimer : CountDownTimer
{
	public event EventHandler<RedeemTimerTickEventArgs> Tick;

	public event EventHandler<RedeemTimerFinishEventArgs> Finish;

	public RedeemTimer(long millisInFuture, long countDownInterval)
		: base(millisInFuture, countDownInterval)
	{
	}

	public override void OnFinish()
	{
		this.Finish?.Invoke(this, new RedeemTimerFinishEventArgs());
	}

	public override void OnTick(long millisUntilFinished)
	{
		this.Tick?.Invoke(this, new RedeemTimerTickEventArgs
		{
			MillisUntilFinished = millisUntilFinished
		});
	}
}
