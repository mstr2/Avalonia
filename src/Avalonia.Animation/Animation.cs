// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Animation.Animators;
using Avalonia.Animation.Easings;
using Avalonia.Collections;
using Avalonia.Data;
using Avalonia.Metadata;

namespace Avalonia.Animation
{
    /// <summary>
    /// Tracks the progress of an animation.
    /// </summary>
    public class Animation : AvaloniaObject, IAnimation
    {
        /// <summary>
        /// Defines the <see cref="Duration"/> property.
        /// </summary>
        public static readonly DirectProperty<Animation, TimeSpan> DurationProperty =
            DependencyProperty.RegisterDirect<Animation, TimeSpan>(
                nameof(_duration),
                o => o._duration,
                (o, v) => o._duration = v);

        /// <summary>
        /// Defines the <see cref="IterationCount"/> property.
        /// </summary>
        public static readonly DirectProperty<Animation, IterationCount> IterationCountProperty =
            DependencyProperty.RegisterDirect<Animation, IterationCount>(
                nameof(_iterationCount),
                o => o._iterationCount,
                (o, v) => o._iterationCount = v);

        /// <summary>
        /// Defines the <see cref="PlaybackDirection"/> property.
        /// </summary>
        public static readonly DirectProperty<Animation, PlaybackDirection> PlaybackDirectionProperty =
            DependencyProperty.RegisterDirect<Animation, PlaybackDirection>(
                nameof(_playbackDirection),
                o => o._playbackDirection,
                (o, v) => o._playbackDirection = v);

        /// <summary>
        /// Defines the <see cref="FillMode"/> property.
        /// </summary>
        public static readonly DirectProperty<Animation, FillMode> FillModeProperty =
            DependencyProperty.RegisterDirect<Animation, FillMode>(
                nameof(_fillMode),
                o => o._fillMode,
                (o, v) => o._fillMode = v);

        /// <summary>
        /// Defines the <see cref="Easing"/> property.
        /// </summary>
        public static readonly DirectProperty<Animation, Easing> EasingProperty =
            DependencyProperty.RegisterDirect<Animation, Easing>(
                nameof(_easing),
                o => o._easing,
                (o, v) => o._easing = v);

        /// <summary>
        /// Defines the <see cref="Delay"/> property.
        /// </summary>
        public static readonly DirectProperty<Animation, TimeSpan> DelayProperty =
            DependencyProperty.RegisterDirect<Animation, TimeSpan>(
                nameof(_delay),
                o => o._delay,
                (o, v) => o._delay = v);

        /// <summary>
        /// Defines the <see cref="DelayBetweenIterations"/> property.
        /// </summary>
        public static readonly DirectProperty<Animation, TimeSpan> DelayBetweenIterationsProperty =
            DependencyProperty.RegisterDirect<Animation, TimeSpan>(
                nameof(_delayBetweenIterations),
                o => o._delayBetweenIterations,
                (o, v) => o._delayBetweenIterations = v);

        /// <summary>
        /// Defines the <see cref="SpeedRatio"/> property.
        /// </summary>
        public static readonly DirectProperty<Animation, double> SpeedRatioProperty =
            DependencyProperty.RegisterDirect<Animation, double>(
                nameof(_speedRatio),
                o => o._speedRatio,
                (o, v) => o._speedRatio = v,
                defaultBindingMode: BindingMode.TwoWay);

        private TimeSpan _duration;
        private IterationCount _iterationCount = new IterationCount(1);
        private PlaybackDirection _playbackDirection;
        private FillMode _fillMode;
        private Easing _easing = new LinearEasing();
        private TimeSpan _delay = TimeSpan.Zero;
        private TimeSpan _delayBetweenIterations = TimeSpan.Zero;
        private double _speedRatio = 1d;

        /// <summary>
        /// Gets or sets the active time of this animation.
        /// </summary>
        public TimeSpan Duration
        {
            get { return _duration; }
            set { SetAndRaise(DurationProperty, ref _duration, value); }
        }

        /// <summary>
        /// Gets or sets the repeat count for this animation.
        /// </summary>
        public IterationCount IterationCount
        {
            get { return _iterationCount; }
            set { SetAndRaise(IterationCountProperty, ref _iterationCount, value); }
        }

        /// <summary>
        /// Gets or sets the playback direction for this animation.
        /// </summary>
        public PlaybackDirection PlaybackDirection
        {
            get { return _playbackDirection; }
            set { SetAndRaise(PlaybackDirectionProperty, ref _playbackDirection, value); }
        }

        /// <summary>
        /// Gets or sets the value fill mode for this animation.
        /// </summary> 
        public FillMode FillMode
        {
            get { return _fillMode; }
            set { SetAndRaise(FillModeProperty, ref _fillMode, value); }
        }

        /// <summary>
        /// Gets or sets the easing function to be used for this animation.
        /// </summary>
        public Easing Easing
        {
            get { return _easing; }
            set { SetAndRaise(EasingProperty, ref _easing, value); }
        }

        /// <summary> 
        /// Gets or sets the initial delay time for this animation. 
        /// </summary> 
        public TimeSpan Delay
        {
            get { return _delay; }
            set { SetAndRaise(DelayProperty, ref _delay, value); }
        }

        /// <summary> 
        /// Gets or sets the delay time in between iterations.
        /// </summary> 
        public TimeSpan DelayBetweenIterations
        {
            get { return _delayBetweenIterations; }
            set { SetAndRaise(DelayBetweenIterationsProperty, ref _delayBetweenIterations, value); }
        }

        /// <summary>
        /// Gets or sets the speed multiple for this animation.
        /// </summary> 
        public double SpeedRatio
        {
            get { return _speedRatio; }
            set { SetAndRaise(SpeedRatioProperty, ref _speedRatio, value); }
        }

        /// <summary>
        /// Obsolete: Do not use this property, use <see cref="IterationCount"/> instead.
        /// </summary>
        /// <value></value>
        [Obsolete("This property has been superceded by IterationCount.")]
        public string RepeatCount
        {
            get { return IterationCount.ToString(); }
            set
            {
                var val = value.ToUpper();
                val = val.Replace("LOOP", "INFINITE");
                val = val.Replace("NONE", "1");
                IterationCount = IterationCount.Parse(val);
            }
        }

        /// <summary>
        /// Gets the children of the <see cref="Animation"/>.
        /// </summary>
        [Content]
        public KeyFrames Children { get; } = new KeyFrames();

        private readonly static List<(Func<DependencyProperty, bool> Condition, Type Animator)> Animators = new List<(Func<DependencyProperty, bool>, Type)>
        {
            ( prop => typeof(bool).IsAssignableFrom(prop.PropertyType), typeof(BoolAnimator) ),
            ( prop => typeof(byte).IsAssignableFrom(prop.PropertyType), typeof(ByteAnimator) ),
            ( prop => typeof(Int16).IsAssignableFrom(prop.PropertyType), typeof(Int16Animator) ),
            ( prop => typeof(Int32).IsAssignableFrom(prop.PropertyType), typeof(Int32Animator) ),
            ( prop => typeof(Int64).IsAssignableFrom(prop.PropertyType), typeof(Int64Animator) ),
            ( prop => typeof(UInt16).IsAssignableFrom(prop.PropertyType), typeof(UInt16Animator) ),
            ( prop => typeof(UInt32).IsAssignableFrom(prop.PropertyType), typeof(UInt32Animator) ),
            ( prop => typeof(UInt64).IsAssignableFrom(prop.PropertyType), typeof(UInt64Animator) ),
            ( prop => typeof(float).IsAssignableFrom(prop.PropertyType), typeof(FloatAnimator) ),
            ( prop => typeof(double).IsAssignableFrom(prop.PropertyType), typeof(DoubleAnimator) ),
            ( prop => typeof(decimal).IsAssignableFrom(prop.PropertyType), typeof(DecimalAnimator) ),
        };

        public static void RegisterAnimator<TAnimator>(Func<DependencyProperty, bool> condition)
            where TAnimator : IAnimator
        {
            Animators.Insert(0, (condition, typeof(TAnimator)));
        }

        private static Type GetAnimatorType(DependencyProperty property)
        {
            foreach (var (condition, type) in Animators)
            {
                if (condition(property))
                {
                    return type;
                }
            }
            return null;
        }

        private (IList<IAnimator> Animators, IList<IDisposable> subscriptions) InterpretKeyframes(Animatable control)
        {
            var handlerList = new List<(Type type, DependencyProperty property)>();
            var animatorKeyFrames = new List<AnimatorKeyFrame>();
            var subscriptions = new List<IDisposable>();

            foreach (var keyframe in Children)
            {
                foreach (var setter in keyframe.Setters)
                {
                    var handler = GetAnimatorType(setter.Property);

                    if (handler == null)
                    {
                        throw new InvalidOperationException($"No animator registered for the property {setter.Property}. Add an animator to the Animation.Animators collection that matches this property to animate it.");
                    }

                    if (!handlerList.Contains((handler, setter.Property)))
                        handlerList.Add((handler, setter.Property));

                    var cue = keyframe.Cue;

                    if (keyframe.TimingMode == KeyFrameTimingMode.TimeSpan)
                    {
                        cue = new Cue(keyframe.KeyTime.Ticks / Duration.Ticks);
                    }

                    var newKF = new AnimatorKeyFrame(handler, cue);

                    subscriptions.Add(newKF.BindSetter(setter, control));

                    animatorKeyFrames.Add(newKF);
                }
            }

            var newAnimatorInstances = new List<IAnimator>();

            foreach (var (handlerType, property) in handlerList)
            {
                var newInstance = (IAnimator)Activator.CreateInstance(handlerType);
                newInstance.Property = property;
                newAnimatorInstances.Add(newInstance);
            }

            foreach (var keyframe in animatorKeyFrames)
            {
                var animator = newAnimatorInstances.First(a => a.GetType() == keyframe.AnimatorType &&
                                                             a.Property == keyframe.Property);
                animator.Add(keyframe);
            }

            return (newAnimatorInstances, subscriptions);
        }

        /// <inheritdocs/>
        public IDisposable Apply(Animatable control, IClock clock, IObservable<bool> match, Action onComplete)
        {
            var (animators, subscriptions) = InterpretKeyframes(control);
            if (animators.Count == 1)
            {
                subscriptions.Add(animators[0].Apply(this, control, clock, match, onComplete));
            }
            else
            {
                var completionTasks = onComplete != null ? new List<Task>() : null;
                foreach (IAnimator animator in animators)
                {
                    Action animatorOnComplete = null;
                    if (onComplete != null)
                    {
                        var tcs = new TaskCompletionSource<object>();
                        animatorOnComplete = () => tcs.SetResult(null);
                        completionTasks.Add(tcs.Task);
                    }
                    subscriptions.Add(animator.Apply(this, control, clock, match, animatorOnComplete));
                }

                if (onComplete != null)
                {
                    Task.WhenAll(completionTasks).ContinueWith(_ => onComplete());
                }
            }
            return new CompositeDisposable(subscriptions);
        }

        /// <inheritdocs/>
        public Task RunAsync(Animatable control, IClock clock = null)
        {
            var run = new TaskCompletionSource<object>();

            if (this.IterationCount == IterationCount.Infinite)
                run.SetException(new InvalidOperationException("Looping animations must not use the Run method."));

            IDisposable subscriptions = null;
            subscriptions = this.Apply(control, clock, Observable.Return(true), () =>
            {
                run.SetResult(null);
                subscriptions?.Dispose();
            });

            return run.Task;
        }
    }
}
