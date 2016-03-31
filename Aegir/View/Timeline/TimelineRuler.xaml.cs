﻿using Aegir.ViewModel.Timeline;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Aegir.View.Timeline
{
    /// <summary>
    /// Interaction logic for KeyframeTimeline.xaml
    /// </summary>
    public partial class TimelineRuler : UserControl
    {
        /// <summary>
        /// Access to logging
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(typeof(TimelineRuler));
        /// <summary>
        /// Visual for the rectangle highlighting the current time
        /// </summary>
        private Rectangle currentTimeHighlighter;
        /// <summary>
        /// Keyframe visuals
        /// </summary>
        [ObsoleteAttribute("Will be removed")]
        private List<Visual> keyframeVisuals;

        /// <summary>
        /// Where the start of our timeline is
        /// </summary>
        public int TimeRangeStart
        {
            get { return (int)GetValue(TimeRangeStartProperty); }
            set
            {
                SetValue(TimeRangeStartProperty, value);
            }
        }

        /// <summary>
        /// TimeRangeStart Dep Property
        /// </summary>
        public static readonly DependencyProperty TimeRangeStartProperty =
            DependencyProperty.Register(nameof(TimeRangeStart),
                                typeof(int),
                                typeof(TimelineRuler),
                                new PropertyMetadata(
                                    0,
                                    new PropertyChangedCallback(TimeRangeChanged)
                                ));

        /// <summary>
        /// Where the end of our timeline is
        /// </summary>
        public int TimeRangeEnd
        {
            get { return (int)GetValue(TimeRangeEndProperty); }
            set
            {
                SetValue(TimeRangeEndProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for Timerange end
        /// </summary>
        public static readonly DependencyProperty TimeRangeEndProperty =
            DependencyProperty.Register(nameof(TimeRangeEnd),
                                typeof(int),
                                typeof(TimelineRuler),
                                new PropertyMetadata(
                                    100,
                                    new PropertyChangedCallback(TimeRangeChanged)
                                ));

        /// <summary>
        /// Currently where in time on our timeline we are
        /// </summary>
        public int CurrentTime
        {
            get { return (int)GetValue(CurrentTimeProperty); }
            set
            {
                SetValue(CurrentTimeProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for our time/position on timeline
        /// </summary>
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register(nameof(CurrentTime),
                                typeof(int),
                                typeof(TimelineRuler),
                                new PropertyMetadata(
                                    0,
                                    new PropertyChangedCallback(CurrentTimeChanged)
                                ));

        /// <summary>
        /// Color of the ticks on the timeline
        /// </summary>
        public Brush TicksColor
        {
            get { return (Brush)GetValue(TicksColorProperty); }
            set
            {
                SetValue(TicksColorProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for our ticks color
        /// </summary>
        public static readonly DependencyProperty TicksColorProperty =
            DependencyProperty.Register(nameof(TicksColor),
                                typeof(Brush),
                                typeof(TimelineRuler),
                                new PropertyMetadata(
                                    new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
                                    new PropertyChangedCallback(TicksColorChanged)
                                ));

        /// <summary>
        /// Color of the rectangle highlighting where in time we are on our timeline
        /// </summary>
        public Brush CurrentTimeHighlightColor
        {
            get { return (Brush)GetValue(CurrentTimeHighlightColorProperty); }
            set
            {
                SetValue(CurrentTimeHighlightColorProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for color of current time highlight rectangle
        /// </summary>
        public static readonly DependencyProperty CurrentTimeHighlightColorProperty =
            DependencyProperty.Register(nameof(CurrentTimeHighlightColor),
                                typeof(Brush),
                                typeof(TimelineRuler),
                                new PropertyMetadata(
                                    new SolidColorBrush(Color.FromArgb(100, 0, 0, 255)),
                                    new PropertyChangedCallback(CurrentTimeHighlightChanged)
                                ));

        /// <summary>
        /// Instanciates a new Keyframe timeline
        /// </summary>
        public TimelineRuler()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Callback for dependency property of the color of our ticks
        /// </summary>
        /// <param name="d">Object changed occured on</param>
        /// <param name="e">Event object for our change</param>
        public static void TicksColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimelineRuler view = d as TimelineRuler;
            if (view != null)
            {
                view.InvalidateTimeline();
            }
        }

        /// <summary>
        /// Callback for highlight rectangle color change
        /// </summary>
        /// <param name="d">Object changed occured on</param>
        /// <param name="e">Event object for our change</param>
        public static void CurrentTimeHighlightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimelineRuler view = d as TimelineRuler;
            if (view != null)
            {
                view.InvalidateCurrentTimeHighlight();
            }
        }

        /// <summary>
        /// Either start or end changed
        /// </summary>
        /// <param name="d">Object changed occured on</param>
        /// <param name="e">Event object for our change</param>
        public static void TimeRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimelineRuler view = d as TimelineRuler;
            if (view != null)
            {
                view.UpdateTimeRange();
            }
        }

        /// <summary>
        /// Callback for dep prop change of current time on timeline
        /// </summary>
        /// <param name="d">Object changed occured on</param>
        /// <param name="e">Event object for our change</param>
        public static void CurrentTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimelineRuler view = d as TimelineRuler;
            if (view != null)
            {
                view.InvalidateCurrentTimeHighlight();
            }
        }

        /// <summary>
        /// Updates the current Time Range and Invalidates the ticks/size of the timeline
        /// </summary>
        public void UpdateTimeRange()
        {
            log.DebugFormat("TimeRange Update: {0} / {1}", TimeRangeStart, TimeRangeEnd);
            InvalidateFullTimeline();
        }

        /// <summary>
        /// Updates the current time on timeline and invalidates the highlight rectangle
        /// </summary>
        public void CurrentTimeChanged()
        {
            InvalidateCurrentTimeHighlight();
        }

        /// <summary>
        /// Called upon control changing size, requiring repositioning of the timeline ticks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            InvalidateFullTimeline();
        }

        /// <summary>
        /// Invalidates all of the timeline parts
        /// </summary>
        private void InvalidateFullTimeline()
        {
            InvalidateTimeline();
            InvalidateCurrentTimeHighlight();
        }

        /// <summary>
        /// Invalidates the position and color of the current time highlight rectangle
        /// </summary>
        private void InvalidateCurrentTimeHighlight()
        {
            if (currentTimeHighlighter == null)
            {
                currentTimeHighlighter = new Rectangle();
            }
            currentTimeHighlighter.Fill = CurrentTimeHighlightColor;
            currentTimeHighlighter.Width = 7;
            currentTimeHighlighter.Height = 22;
            currentTimeHighlighter.Stroke = new SolidColorBrush(Color.FromArgb(255, 128, 128, 128));
            currentTimeHighlighter.StrokeThickness = 1;
            if (!KeyFrameTimeLineRuler.Children.Contains(currentTimeHighlighter))
            {
                KeyFrameTimeLineRuler.Children.Add(currentTimeHighlighter);
            }
            double stepSize = (ActualWidth - 20) / (TimeRangeEnd - TimeRangeStart);
            double leftOffset = stepSize * CurrentTime + 10;
            Canvas.SetLeft(currentTimeHighlighter, leftOffset - 4);
            Canvas.SetTop(currentTimeHighlighter, 0);
        }

        /// <summary>
        /// Invalidates the size of the timeline
        /// </summary>
        private void InvalidateTimeline()
        {
            //Get range
            int range = (TimeRangeEnd - TimeRangeStart) + 1;
            //For now always have 5 segments
            double segmentSize = (ActualWidth / range) * 10;

            //Align ticks/viewport with range and width
            //KeyFrameTimeLineRuler.Background = ticksBrush;
            //Set Ruler Thickness
            int numOfSegments = (int)Math.Ceiling(range / 10d);
            GenerateTickSegments(range, numOfSegments, 10);
        }

        /// <summary>
        /// Generates tick visuals
        /// </summary>
        /// <param name="range">The size of franes in the timeline (I.E end-start)</param>
        /// <param name="numOfSegments">Number of segments to create in this range</param>
        /// <param name="numOfSegmentSteps">How many ticks in one segment</param>
        private void GenerateTickSegments(int range, int numOfSegments, int numOfSegmentSteps)
        {
            double keyFrameTicksPadding = 10;
            KeyFrameTimeLineRuler.Children.Clear();
            double stepSize = (ActualWidth - keyFrameTicksPadding * 2) / (range - 1);
            //if (range > 40)
            //{
            //    while (!(stepSize < 10 && stepSize > 2))
            //    {
            //        if (stepSize > 10)
            //        {
            //            stepSize = stepSize / 2;
            //        }
            //        if (stepSize < 2)
            //        {
            //            stepSize = stepSize * 2;
            //        }
            //    }
            //}
            Line[] tickLines = new Line[range];
            for (int i = 0; i < range; i++)
            {
                double xOffset = i * stepSize + keyFrameTicksPadding;
                //Last tick of segment
                Line tickLine = new Line();
                tickLine.X1 = xOffset;
                tickLine.X2 = xOffset;
                tickLine.Y1 = 0;
                if (i % numOfSegmentSteps == 0)
                {
                    tickLine.Y2 = 20;
                }
                else
                {
                    tickLine.Y2 = 10;
                }
                tickLine.Stroke = TicksColor;
                tickLines[i] = tickLine;

                this.KeyFrameTimeLineRuler.Children.Add(tickLine);
            }
        }
    }
}