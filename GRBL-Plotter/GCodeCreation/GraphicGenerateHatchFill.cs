/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2023 Sven Hasemann contact: svenhb@web.de

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 * 2020-07-05 Code adapted from Evil Mad Scientist eggbot_hatch.py
 * 2021-07-14 code clean up / code quality
 * 2022-11-04 line 57 set dash pattern to 0
 * 2023-01-15 line 90 bug-fix in log-output
*/


using System;
using System.Collections.Generic;
using System.Windows;

namespace GrblPlotter
{
    public static partial class Graphic
    {
        internal static void HatchFill(List<PathObject> graphicToFill)
        {
            if (graphicToFill == null) return;

            double distance = (double)Properties.Settings.Default.importGraphicHatchFillDistance;
            double angle = (double)Properties.Settings.Default.importGraphicHatchFillAngle;
            double angle2 = (double)Properties.Settings.Default.importGraphicHatchFillAngle2;
            bool cross = Properties.Settings.Default.importGraphicHatchFillCross;
            bool incrementAngle = Properties.Settings.Default.importGraphicHatchFillAngleInc;

            bool nextIsSameHatch;
            bool fillColor = graphicInformation.ApplyHatchFill;	// only hatch if fillColor is set

            bool applyDash = Properties.Settings.Default.importGraphicHatchFillDash;
            int maxObject = graphicToFill.Count;

            Logger.Trace("...HatchFill objects:{0}  distance:{1} angle:{2} cross:{3}  dash:{4}", maxObject, distance, angle, cross, applyDash);

            List<Point[]> hatchPattern = new List<Point[]>();
            List<Point[]> finalPattern = new List<Point[]>();

            List<PathObject> tmpPath = new List<PathObject>();
            Dimensions pathDimension = new Dimensions();

            if (!applyDash)
            { SetDash(new double[0]); }

            for (int index = 0; index < maxObject; index++)
            {
                PathObject itemNow = graphicToFill[index];
            //    ItemPath itemNext;
                if (itemNow is ItemPath PathData)
                {
                    nextIsSameHatch = false;
                    if ((index < (maxObject - 1)) && (graphicToFill[index + 1] is ItemPath PathDataNext))
                    { nextIsSameHatch = ((PathData.Info.Id == PathDataNext.Info.Id) && (IsEqual(PathDataNext.Start, PathDataNext.End) && (PathDataNext.Path.Count > 2))); }

                    if (IsEqual(PathData.Start, PathData.End) && (PathData.Path.Count > 2))      //(PathData.Start.X == PathData.End.X) && (PathData.Start.Y == PathData.End.Y))
                    {
                        string fill = PathData.Info.GroupAttributes[(int)GroupOption.ByFill];
                        if (fillColor && ((string.IsNullOrEmpty(fill)) || (fill == "none")))	// SVG: only hatch if fillColor is set
                        {   //Logger.Trace("no fill");
                            continue;
                        }
                        else
                            CountProperty((int)GroupOption.ByColor, fill);      // now fill-color is also penColor -> for grouping

                        tmpPath.Add(PathData);                  // collect paths
                        pathDimension.AddDimensionXY(PathData.Dimension);   // adapt overall size
                        if (!pathDimension.IsXYSet())						// no dimension - nothing to do
                        { Logger.Trace("no dim"); continue; }

                        // collect paths of same id, process if id changes
                        if (logModification && (index < (maxObject - 1))) Logger.Trace("  Add to PathData ID:{0}  nextIsSameHatch:{1}  max:{2}  index:{3}  id_now:{4}  id_next:{5}  fill:{6}", PathData.Info.Id, nextIsSameHatch, maxObject, index, graphicToFill[index].Info.Id, graphicToFill[index + 1].Info.Id, fill);
                        if (nextIsSameHatch)
                        { continue; }

                        // create hatch pattern
                        hatchPattern.Clear();
                        hatchPattern.AddRange(CreateLinePattern(pathDimension, angle, distance));
                        if (cross)
                            hatchPattern.AddRange(CreateLinePattern(pathDimension, angle + 90, distance));

                        if (incrementAngle) angle += angle2;

                        // process single hatch lines - shorten to match inside polygone
                        finalPattern.Clear();

                        foreach (Point[] hatchLine in hatchPattern)
                        { ClipLineByPolygone(hatchLine[0], hatchLine[1], tmpPath, finalPattern); }

                        // add processed hatch lines to final graphic
                        AddLinesToGraphic(finalPattern, PathData);

                        // tidy up for next object with new id
                        tmpPath.Clear();
                        pathDimension = new Dimensions();
                    }
                }
                else
                {
                    if (logModification) Logger.Trace(" is Dot ID:{0} Length:{1:0.00}  start x:{2:0.00} y:{3:0.00} end x:{4:0.00} y:{5:0.00} ", itemNow.Info.Id, itemNow.PathLength, itemNow.Start.X, itemNow.Start.Y, itemNow.End.X, itemNow.End.Y);
                }
            }
            if (logModification) Logger.Trace("HatchFill End --------------------------------------", actualDimension.minx, actualDimension.miny);
        }


        private static List<Point[]> CreateLinePattern(Dimensions dim, double angle, double distance)
        { return CreateLinePattern(dim.minx, dim.miny, dim.maxx, dim.maxy, angle, distance); }
        private static List<Point[]> CreateLinePattern(double minx, double miny, double maxx, double maxy, double angle, double distance)
        {
            double width = maxx - minx;
            double height = maxy - miny;
            double r = Math.Sqrt(width * width + height * height) / 2;

            // Rotation information
            double ca = Math.Cos((angle - 90) * Math.PI / 180);
            double sa = Math.Sin((angle - 90) * Math.PI / 180);

            // Translation information
            double cx = minx + (width / 2);
            double cy = miny + (height / 2);

            double x1, y1, x2, y2;
            List<Point[]> lines = new List<Point[]>();

            //     int count = 0;
            for (double i = -r; i < r; i += distance)
            {
                x1 = cx + (i * ca) + (r * sa);//  # i * ca - (-r) * sa
                y1 = cy + (i * sa) - (r * ca);  //# i * sa + (-r) * ca
                x2 = cx + (i * ca) - (r * sa);  //# i * ca - (+r) * sa
                y2 = cy + (i * sa) + (r * ca);  //# i * sa + (+r) * ca
                                                // Remove any potential hatch lines which are entirely
                                                // outside of the bounding box
                if ((x1 < minx && x2 < minx) || (x1 > maxx && x2 > maxx))
                    continue;
                if ((y1 < miny && y2 < miny) || (y1 > maxy && y2 > maxy))
                    continue;
                lines.Add(new Point[] { new Point(x1, y1), new Point(x2, y2) });
            }
            return lines;
        }

        private static void AddLinesToGraphic(List<Point[]> HatchLines, ItemPath PathData)
        {
            if (logModification) Logger.Trace("  AddLinesToGraphic");
            Point start, end;
            bool switchColor = graphicInformation.ApplyHatchFill;
            for (int i = 0; i < HatchLines.Count; i++)
            {
                if ((i % 2) > 0)
                { start = HatchLines[i][0]; end = HatchLines[i][1]; }
                else
                { start = HatchLines[i][1]; end = HatchLines[i][0]; }

                StartPath(start);

                actualPath.Info.Id = PathData.Info.Id;
                actualPath.Info.CopyData(PathData.Info);    // preset global info for GROUP
                if (switchColor)
                    actualPath.Info.GroupAttributes[(int)GroupOption.ByColor] = actualPath.Info.GroupAttributes[(int)GroupOption.ByFill];
                //        actualPath.Info.PathGeometry += "_hatch";
                actualPath.Info.PathGeometry = "hatch_fill_" + actualPath.Info.PathGeometry;

                AddLine(end);
                StopPath();
            }
        }

        private static double CalculateIntersection(Point p1, Point p2, Point p3, Point p4)
        {
            double d21x = p2.X - p1.X;
            double d21y = p2.Y - p1.Y;
            double d43x = p4.X - p3.X;
            double d43y = p4.Y - p3.Y;

            double d = d21x * d43y - d21y * d43x;

            if (d == 0)
                return -1.0;

            double nb = (p1.Y - p3.Y) * d21x - (p1.X - p3.X) * d21y;

            double sb = nb / d;
            if ((sb < 0) || (sb > 1))
                return -1.0;

            double na = (p1.Y - p3.Y) * d43x - (p1.X - p3.X) * d43y;
            double sa = na / d;
            if ((sa < 0) || (sa > 1))
                return -1.0;

            return sa;
        }

        internal struct IntersectionInfo
        {
            public double s;
            public double s1;
            public double s2;
            public IntersectionInfo(double s, double s1, double s2)
            {
                this.s = s;
                this.s1 = s1;
                this.s2 = s2;
            }
        };
        // process single hatch lines - shorten to match inside polygone
        private static void ClipLineByPolygone(Point p1, Point p2, List<PathObject> paths, List<Point[]> hatch)//, hatches, b_hold_back_hatches, f_hold_back_steps):
        {
            Point p3, p4;
            double intersect;

            List<IntersectionInfo> d_and_a = new List<IntersectionInfo>();

            double holdBack = (double)Properties.Settings.Default.importGraphicHatchFillInset;
            bool holdBackEnable = Properties.Settings.Default.importGraphicHatchFillInsetEnable;
            bool b_unconditionally_excise_hatch;
            double prelim_length_to_be_removed = 0;
            double dist_intersection_to_relevant_end, dist_intersection_to_irrelevant_end;

            foreach (PathObject path in paths)
            {
                if (path is ItemPath ipath)
                {
                    p3 = ipath.Path[0].MoveTo;

                    for (int k = 1; k < ipath.Path.Count; k++)
                    {
                        p4 = ipath.Path[k].MoveTo;
                        intersect = CalculateIntersection(p1, p2, p3, p4);
                        if ((0.0 <= intersect) && (intersect <= 1.0))
                        {

                            if (holdBackEnable)    // shorten the hatch lines
                            {
                                double angle_hatch_radians = Math.Atan2(-(p2.Y - p1.Y), (p2.X - p1.X));  //# from p1 toward p2, cartesian coordinates
                                double angle_segment_radians = Math.Atan2(-(p4.Y - p3.Y), (p4.X - p3.X));  //# from p3 toward p4, cartesian coordinates
                                double angle_difference_radians = angle_hatch_radians - angle_segment_radians;

                                if (angle_difference_radians > Math.PI)
                                    angle_difference_radians -= 2 * Math.PI;
                                else if (angle_difference_radians < -Math.PI)
                                    angle_difference_radians += 2 * Math.PI;
                                double f_sin_of_join_angle = Math.Sin(angle_difference_radians);
                                double f_abs_sin_of_join_angle = Math.Abs(f_sin_of_join_angle);
                                if (f_abs_sin_of_join_angle != 0.0) //  # Worrying about case of intersecting a segment parallel to the hatch
                                {
                                    prelim_length_to_be_removed = holdBack / f_abs_sin_of_join_angle;
                                    b_unconditionally_excise_hatch = false;
                                }
                                else
                                    b_unconditionally_excise_hatch = true;

                                if (!b_unconditionally_excise_hatch)
                                {    //# The relevant end of the segment is the end from which the hatch approaches at an acute angle.
                                    Point intersection = new Point
                                    {
                                        X = p1.X + intersect * (p2.X - p1.X),  //# compute intersection point of hatch with segment
                                        Y = p1.Y + intersect * (p2.Y - p1.Y) // # intersecting hatch line starts at p1, vectored toward p2,
                                    };

                                    if (Math.Abs(angle_difference_radians) < Math.PI / 2)
                                    {    //# It's near the p3 the relevant end from which the hatch departs
                                        dist_intersection_to_relevant_end = CalcHypotenuse(p3.X - intersection.X, p3.Y - intersection.Y);
                                        dist_intersection_to_irrelevant_end = CalcHypotenuse(p4.X - intersection.X, p4.Y - intersection.Y);
                                    }
                                    else
                                    {    //# It's near the p4 end from which the hatch departs
                                        dist_intersection_to_relevant_end = CalcHypotenuse(p4.X - intersection.X, p4.Y - intersection.Y);
                                        dist_intersection_to_irrelevant_end = CalcHypotenuse(p3.X - intersection.X, p3.Y - intersection.Y);
                                    }

                                    double length_remove_starting_hatch = prelim_length_to_be_removed;
                                    double length_remove_ending_hatch = prelim_length_to_be_removed;

                                    //# Now check each of the two ends
                                    if (prelim_length_to_be_removed > (dist_intersection_to_relevant_end + holdBack))
                                        //# Yes, would be excessive holdback approaching from this direction
                                        length_remove_starting_hatch = dist_intersection_to_relevant_end + holdBack;
                                    if (prelim_length_to_be_removed > (dist_intersection_to_irrelevant_end + holdBack))
                                        //# Yes, would be excessive holdback approaching from other direction
                                        length_remove_ending_hatch = dist_intersection_to_irrelevant_end + holdBack;

                                    d_and_a.Add(new IntersectionInfo(intersect, length_remove_starting_hatch, length_remove_ending_hatch));

                                }
                                else
                                    d_and_a.Add(new IntersectionInfo(intersect, 123456.0, 123456.0));//  # Mark for complete hatch excision, hatch is parallel to segment
                            }
                            else
                                d_and_a.Add(new IntersectionInfo(intersect, 0, 0));//  # zero length to be removed from hatch										
                        }
                        p3 = p4;
                    }
                }
            }
            //# Return now if there were no intersections
            if (d_and_a.Count == 0)
                return;

            // d_and_a.sort() - by s
            d_and_a.Sort((x, y) => x.s.CompareTo(y.s));

            // Remove duplicate intersections
            int n = d_and_a.Count;
            int i_last = 1;
            int i = 1;
            IntersectionInfo last = d_and_a[0];
            while (i < n)
            {
                if ((Math.Abs(d_and_a[i].s - last.s)) > 0.0000000001)   //F_MINGAP_SMALL_VALUE)	// 0.0000000001  
                {
                    d_and_a[i_last] = last = d_and_a[i];
                    i_last += 1;
                }
                i += 1;
            }

            d_and_a = d_and_a.GetRange(0, i_last);
            if (d_and_a.Count < 2)
                return;


            int j = 0;
            double x1, y1, x2, y2;
            double distance = (double)Properties.Settings.Default.importGraphicHatchFillDistance;
            double f_min_allowed_hatch_length, f_initial_hatch_length, f_length_to_be_removed_from_pt1, f_length_to_be_removed_from_pt2;

            while (j < (d_and_a.Count - 1))
            {
                x1 = p1.X + d_and_a[j].s * (p2.X - p1.X);
                y1 = p1.Y + d_and_a[j].s * (p2.Y - p1.Y);
                x2 = p1.X + d_and_a[j + 1].s * (p2.X - p1.X);
                y2 = p1.Y + d_and_a[j + 1].s * (p2.Y - p1.Y);

                if (!holdBackEnable)
                    hatch.Add(new Point[] { new Point(x1, y1), new Point(x2, y2) });
                else
                {
                    f_min_allowed_hatch_length = distance * 0.25;   //MIN_HATCH_FRACTION; # Minimum hatch length, as a fraction of the hatch spacing.
                    f_initial_hatch_length = CalcHypotenuse(x2 - x1, y2 - y1);
                    f_length_to_be_removed_from_pt1 = d_and_a[j].s2;
                    f_length_to_be_removed_from_pt2 = d_and_a[j + 1].s1;
                    if ((f_initial_hatch_length - (f_length_to_be_removed_from_pt1 + f_length_to_be_removed_from_pt2)) <= f_min_allowed_hatch_length)
                    { }     //  # Just don't insert it into the hatch list
                    else
                    {
                        Point pt1 = RelativeControlPointPosition(f_length_to_be_removed_from_pt1, x2 - x1, y2 - y1, x1, y1);
                        Point pt2 = RelativeControlPointPosition(f_length_to_be_removed_from_pt2, x1 - x2, y1 - y2, x2, y2);
                        hatch.Add(new Point[] { pt1, pt2 });
                    }
                }
                j += 2;
            }
        }

        private static double CalcHypotenuse(double a, double b)
        { return Math.Sqrt(a * a + b * b); }


        private static Point RelativeControlPointPosition(double distance, double f_delta_x, double f_delta_y, double delta_x, double delta_y)
        {
            //# returns the point, relative to 0, 0 offset by delta_x, delta_y,
            //# which extends a distance of "distance" at a slope defined by f_delta_x and f_delta_y
            Point pt_return = new Point();

            if (f_delta_x == 0)
            {
                pt_return.X = delta_x;
                pt_return.Y = distance * Math.Sign(f_delta_y) + delta_y;
            }
            else if (f_delta_y == 0)
            {
                pt_return.X = distance * Math.Sign(f_delta_x) + delta_x;
                pt_return.Y = delta_y;
            }
            else
            {
                double f_slope = Math.Atan2(f_delta_y, f_delta_x);
                pt_return.X = distance * Math.Cos(f_slope) + delta_x;
                pt_return.Y = distance * Math.Sin(f_slope) + delta_y;
            }

            return pt_return;
        }
    }
}
