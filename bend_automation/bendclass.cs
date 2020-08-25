
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;

namespace bend_automation
{

    public class bendclass
    {
        private static int blueColorIndex = 5;
        private static int greenColorIndex = 3;
        private static double pipeRadius = 100;
        private static double bendAngle = 90;

        enum Direction
        {
            Horizontal,
            Vertical
        }


        [CommandMethod("CreateBend")]
        public static void CreateBend()
        {

            CreateLayer("IE_Object_Layer");
            CreateLayer("IE_Text_Layer");

            double length_D1 = 1000;
            double length_D2 = 1000;
            double length_F = 500;
            
        
            double coOrd1 = 100;
            double coOrd2 = 100;
            

            bendAngle = GetInputFromUser("Bend Angle");
            length_D1 = GetInputFromUser("Length D1");
            length_D2 = GetInputFromUser("Length D2");
            length_F = GetInputFromUser("Length F");
            double pipeDia = GetInputFromUser("Pipe Diameter");

            pipeRadius = pipeDia / 2;

            if (length_D1 > length_F && length_D2 > length_F && pipeRadius <length_F && (bendAngle ==45 || bendAngle ==90))
            {
            
                Point3d pos1=new Point3d();
                Point3d pos2 = new Point3d();
                Point3d pos3 = new Point3d();
                Point3d pos4 = new Point3d();
                Point3d arcCenter = new Point3d();
                Point3d dimPoint = new Point3d();
                Point3d elbowStrPos1 = new Point3d();
                Point3d elbowStrPos2 = new Point3d();
                Point3d elbowEndPos1 = new Point3d();
                Point3d elbowEndPos2 = new Point3d();
                Point3d textPos = new Point3d();
                Vector3d vcTan = new Point3d (0.5,0.5,0).GetAsVector();

                double arcRadius = 0;
                double arcStrtAngle = 0;
                double arcEndAngle = 0;
                string angleText = "";

                Point3d [] splineEndPoints= { };
                
                if (bendAngle == 90)
                {
                    pos1 = new Point3d(coOrd1, coOrd2, 0);
                    pos2 = new Point3d(coOrd1, coOrd2 + length_D1  - length_F , 0);
                    pos3 = new Point3d(coOrd1 + length_F , coOrd2 + length_D1 , 0);
                    pos4  = new Point3d(coOrd1 + length_D2 , coOrd2 + length_D1 , 0);
                    dimPoint = new Point3d(coOrd1, coOrd2 + length_D1, 0);

                    elbowStrPos1  = new Point3d(pos2.X + pipeRadius , pos2.Y, 0);
                    elbowStrPos2  = new Point3d(pos2.X - pipeRadius, pos2.Y, 0);

                    elbowEndPos1 = new Point3d(pos3.X, pos3.Y + pipeRadius, 0);
                    elbowEndPos2 = new Point3d(pos3.X, pos3.Y - pipeRadius, 0);

                    arcCenter   = new Point3d(coOrd1 + length_F , coOrd2 + length_D1  - length_F , 0);
                    arcRadius = length_F;
                    arcStrtAngle = 0.5 * System.Math.PI;
                    arcEndAngle = System.Math.PI;
                    angleText = "90° Bend";                   
                    textPos = arcCenter;
                     
                }
                else if (bendAngle == 45)
                {
                    Double angRad = bendAngle * System .Math . PI / 180;
                    Double halfAng = angRad / 2;

                    pos1 = new Point3d(coOrd1, coOrd2, 0);
                    pos2 = new Point3d(coOrd1, coOrd2 + length_D1  - length_F , 0);
                    pos3 = new Point3d(coOrd1 + (length_F  *System .Math . Cos(angRad)),
                        coOrd2 + length_D1  + (System.Math.Sin(angRad) * length_F ), 0);
                    pos4  = new Point3d(coOrd1 + (length_D2  * System.Math.Cos(angRad)),
                        coOrd2 + length_D1  + (System.Math.Sin(angRad) * length_D2), 0);

                    dimPoint = new Point3d(coOrd1 , coOrd2 + length_D1, 0);

                    textPos = new Point3d(dimPoint.X + pipeRadius * 2, dimPoint.Y, 0);
                    arcRadius = length_F / (System.Math.Tan(angRad / 2));

                    double outerF = System.Math.Tan (halfAng) * (arcRadius  + pipeRadius);
                    double innerF  = System.Math.Tan (halfAng) * (arcRadius  - pipeRadius);

                    double outerD1  = length_D1  - length_F  + outerF;
                    double innerD1  = length_D1 - length_F + innerF;

                    double outerD2  = length_D2 - length_F + outerF;
                    double innerD2  = length_D2 - length_F + innerF;

                    elbowStrPos1 = new Point3d(pos2.X + pipeRadius, pos2.Y, 0);
                    elbowStrPos2 = new Point3d(pos2.X - pipeRadius, pos2.Y, 0);

                    elbowEndPos1 = new Point3d((coOrd1 - pipeRadius) + (outerF * System.Math.Cos(angRad)), 
                        coOrd2 + outerD1 + (System.Math.Sin(angRad) * outerF), 0);
                    elbowEndPos2 = new Point3d((coOrd1 + pipeRadius) + (innerF * System.Math.Cos(angRad)), 
                        coOrd2 + innerD1 + (System.Math.Sin(angRad) * innerF), 0);

                    arcCenter = new Point3d(pos2.X + arcRadius , pos2.Y, 0);

                    arcStrtAngle = 0.75 * System.Math.PI;
                    arcEndAngle = System.Math.PI;
                    angleText = "45° Bend";
                   
                    Point3d pos7 = new Point3d((coOrd1 - pipeRadius) + (outerD2 * System.Math.Cos(angRad)),
                        coOrd2 + outerD1 + (System.Math.Sin(angRad) * outerD2), 0);
                    Point3d pos8 = new Point3d((coOrd1 + pipeRadius) + (innerD2 * System.Math.Cos(angRad)),
                        coOrd2 + innerD1 + (System.Math.Sin(angRad) * innerD2), 0);

                    Point3d[] endPts = { pos7, pos8 };
                    splineEndPoints = endPts;
                    
                }


                CreateLine(pos1, pos2, greenColorIndex, true,pipeRadius );
                CreateArc (arcCenter, arcRadius , arcStrtAngle , arcEndAngle , true, pipeRadius);
                CreateLine(pos3, pos4, greenColorIndex, true, pipeRadius);
                CreateLine(elbowStrPos1, elbowStrPos2, blueColorIndex , false, pipeRadius);
                CreateLine(elbowEndPos1 , elbowEndPos2 , blueColorIndex, false, pipeRadius);
               
                Point3dCollection horSplinePoints = CollectSplinePoints(pos1, splineEndPoints, Direction.Horizontal);
                CreateSpline(horSplinePoints, vcTan);

                Point3dCollection vertSplinePoints = CollectSplinePoints(pos4, splineEndPoints, Direction.Vertical);
                CreateSpline(vertSplinePoints, vcTan);

                Point3d[] horDimPos = { pos3 , pos4 };
                CreateDimension(dimPoint, horDimPos, Direction.Horizontal);
               
                Point3d[] vertDimPos ={ pos2 ,pos1};
                CreateDimension(dimPoint, vertDimPos, Direction.Vertical);

                CreateText(textPos, angleText);

            }
            else 
            {
                if (length_D1 < length_F || length_D2 < length_F)
                {
                    Application.ShowAlertDialog("Length D1 and Length D2 should be greater than Length F And Pipe Radius shoud be lesser than Length F");
                }
                else 
                {
                    Application.ShowAlertDialog(" Bend Angle should be 45° or 90°");
                }
                
            }                          


        }    

        private static void CreateLine(Point3d pos1, Point3d pos2, int ColorInd, bool createOffset,double offDist)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;

            Database dBase = doc.Database;

            using (Transaction acTrans = doc.TransactionManager.StartTransaction())
            {
                BlockTable blockTable1;
                blockTable1 = acTrans.GetObject(dBase.BlockTableId, OpenMode.ForRead) as BlockTable;


                BlockTableRecord recBT;
                recBT = acTrans.GetObject(blockTable1[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Line acLine = new Line(pos1, pos2);

                acLine.SetDatabaseDefaults();
                acLine.Layer = "IE_Object_Layer";

                acLine.ColorIndex = ColorInd;

                recBT.AppendEntity(acLine);

                acTrans.AddNewlyCreatedDBObject(acLine, true);

                if  (createOffset==true ) 
                {
                    CreateOffsetLine(acLine, offDist, recBT, acTrans);
                    CreateOffsetLine(acLine, -offDist, recBT, acTrans);

                    acLine.ColorIndex = 0;

                }
                                          

                acTrans.Commit();

            }

        }

        private static  void CreateOffsetLine(Line lineToOffset ,double offstDist , BlockTableRecord recBT, Transaction acTrans)
        {
         
            DBObjectCollection objColl= lineToOffset.GetOffsetCurves(offstDist);

            foreach (Entity acEnt in objColl)
            {
                recBT.AppendEntity(acEnt);
                acTrans.AddNewlyCreatedDBObject(acEnt, true);
            
            }
        }

        private static void CreateArc( Point3d Center,  Double radius,  Double strtAng, Double endAng,
                         Boolean createOffset,double offDist)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;

            Database dBase = doc.Database;

            using (Transaction acTrans = doc.TransactionManager.StartTransaction())
            {
                BlockTable blockTable1;
                blockTable1 = acTrans.GetObject(dBase.BlockTableId, OpenMode.ForRead) as BlockTable;


                BlockTableRecord recBT;
                recBT = acTrans.GetObject(blockTable1[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Arc acArc = new Arc(Center, radius, strtAng, endAng);

                acArc.SetDatabaseDefaults();
                acArc.Layer = "IE_Object_Layer";

                acArc.ColorIndex = 5;

                recBT.AppendEntity(acArc);

                acTrans.AddNewlyCreatedDBObject(acArc, true);

                if (createOffset == true)
                {
                    CreateOffsetArc (acArc , offDist , recBT, acTrans);
                    CreateOffsetArc (acArc , -offDist , recBT, acTrans);
                    acArc.ColorIndex = 0;

                }

                acTrans.Commit();

            }

        }

        private static void CreateOffsetArc(Arc arcToOffset, double offstDist, BlockTableRecord recBT, Transaction acTrans)
        {

            DBObjectCollection objColl = arcToOffset.GetOffsetCurves(offstDist);

            foreach (Entity acEnt in objColl)
            {
                recBT.AppendEntity(acEnt);
                acTrans.AddNewlyCreatedDBObject(acEnt, true);

            }
        }

        private static void CreateSpline(Point3dCollection pointCol , Vector3d vcTan)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;

            Database dBase = doc.Database;

            using (Transaction acTrans = doc.TransactionManager.StartTransaction())
            {
                BlockTable blockTable1;
                blockTable1 = acTrans.GetObject(dBase.BlockTableId, OpenMode.ForRead) as BlockTable;


                BlockTableRecord recBT;
                recBT = acTrans.GetObject(blockTable1[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Spline  acSpline = new Spline(pointCol, vcTan, vcTan, 7, 0);

                acSpline.SetDatabaseDefaults();
                acSpline.Layer = "IE_Object_Layer";

                acSpline.ColorIndex = 3;

                recBT.AppendEntity(acSpline);

                acTrans.AddNewlyCreatedDBObject(acSpline, true);
           
                acTrans.Commit();

            }

        }

        private static Point3dCollection CollectSplinePoints(Point3d splineMidPoint,Point3d [] splineEndPoints, Direction splineDir)
        {

            Point3dCollection pointCol = new Point3dCollection();

            if (splineDir == Direction.Horizontal)
            {
                pointCol.Add(new Point3d(splineMidPoint.X - pipeRadius, splineMidPoint.Y, 0));
                pointCol.Add(new Point3d(splineMidPoint.X - pipeRadius / 2, splineMidPoint.Y - pipeRadius / 4, 0));
                pointCol.Add(splineMidPoint);
                pointCol.Add(new Point3d(splineMidPoint.X + pipeRadius / 2, splineMidPoint.Y + pipeRadius / 4, 0));
                pointCol.Add(new Point3d(splineMidPoint.X + pipeRadius, splineMidPoint.Y, 0));
                pointCol.Add(new Point3d(splineMidPoint.X + pipeRadius / 2, splineMidPoint.Y - pipeRadius / 4, 0));
                pointCol.Add(splineMidPoint);
            }
            else
            {
                if (bendAngle == 90)
                {

                    pointCol.Add(new Point3d(splineMidPoint.X, splineMidPoint.Y + pipeRadius, 0));
                    pointCol.Add(new Point3d(splineMidPoint.X - pipeRadius / 4, splineMidPoint.Y + pipeRadius / 2, 0));
                    pointCol.Add(splineMidPoint);
                    pointCol.Add(new Point3d(splineMidPoint.X + pipeRadius / 4, splineMidPoint.Y - pipeRadius / 2, 0));
                    pointCol.Add(new Point3d(splineMidPoint.X, splineMidPoint.Y - pipeRadius, 0));
                    pointCol.Add(new Point3d(splineMidPoint.X - pipeRadius / 4, splineMidPoint.Y - pipeRadius / 2, 0));
                    pointCol.Add(splineMidPoint);
                  
                }
                else
                {

                    pointCol.Add(splineEndPoints[0]);
                    pointCol.Add(new Point3d(splineEndPoints[0].X + pipeRadius / 4, splineEndPoints[0].Y - pipeRadius / 2, 0));
                    pointCol.Add(splineMidPoint);
                    pointCol.Add(new Point3d(splineMidPoint.X + pipeRadius / 2, splineEndPoints[1].Y + pipeRadius / 2, 0));
                    pointCol.Add(splineEndPoints[1]);
                    pointCol.Add(new Point3d(splineEndPoints[1].X - pipeRadius / 2, splineMidPoint.Y - pipeRadius / 2, 0));
                    pointCol.Add(splineMidPoint);
                    
                }
                       
            }

            return pointCol;
        }

        private static void CreateDimension(Point3d CommonDimPos  , Point3d[] dimPos,
                                            Direction dimDir)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;

            Database dBase = doc.Database;

            using (Transaction acTrans = doc.TransactionManager.StartTransaction())
            {
                BlockTable blockTable1;
                blockTable1 = acTrans.GetObject(dBase.BlockTableId, OpenMode.ForRead) as BlockTable;


                BlockTableRecord recBT;
                recBT = acTrans.GetObject(blockTable1[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;


                for (int i = 0; i < +dimPos.Length; i++)
                {

                    Point3d dimOrigin = new Point3d( );
                    double rotationAng=0;

                    double offsetFactor = i + 1.5;
                                     
                    if (dimDir == Direction.Horizontal)
                    {
                        dimOrigin = new Point3d((CommonDimPos.X + dimPos[i].X) / 2, dimPos[i].Y + offsetFactor * pipeRadius, 0);
                        
                        if (bendAngle == 90)
                        {
                            rotationAng = System.Math.PI;                            
                        }
                        else
                        {
                            rotationAng = 1.25 * System.Math.PI;
                        }
                    }
                    else
                    {
                        dimOrigin = new Point3d(CommonDimPos.X - offsetFactor * pipeRadius, (dimPos[i].Y + dimPos[i].Y) / 2, 0);
                                              
                        rotationAng = 0.5 * System.Math.PI; 
                       
                    }

                    


                    RotatedDimension rotatedDim = new RotatedDimension();

                    rotatedDim.SetDatabaseDefaults();
                    rotatedDim.XLine1Point = CommonDimPos;
                    rotatedDim.XLine2Point = dimPos[i];
                    rotatedDim.DimLinePoint = dimOrigin;
                    rotatedDim.DimensionStyle = dBase.Dimstyle;
                    rotatedDim.Rotation = rotationAng;
                    rotatedDim.Layer = "IE_Object_Layer";

                    recBT.AppendEntity(rotatedDim);

                    acTrans.AddNewlyCreatedDBObject(rotatedDim, true);

                }
                    
                acTrans.Commit();

            }

        }

        private static void CreateText(Point3d position , String text)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;

            Database dBase = doc.Database;

            using (Transaction acTrans = doc.TransactionManager.StartTransaction())
            {
                BlockTable blockTable1;
                blockTable1 = acTrans.GetObject(dBase.BlockTableId, OpenMode.ForRead) as BlockTable;


                BlockTableRecord recBT;
                recBT = acTrans.GetObject(blockTable1[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                             
                MText objText   = new MText();

                objText.SetDatabaseDefaults();

                objText.Location = position;

                objText.Contents = text;

                objText.TextStyleId = dBase.Textstyle;

                objText.Layer = "IE_Text_Layer";

                recBT.AppendEntity(objText);

                acTrans.AddNewlyCreatedDBObject(objText, true);

                acTrans.Commit();

            }

        }
        private static double GetInputFromUser(string propName)
        {

            double userInput = 0;

            Document acdoc = Application.DocumentManager.MdiActiveDocument;

            PromptStringOptions prmtStr = new PromptStringOptions("Enter " + propName + " : ");

            prmtStr.AllowSpaces = true;

            PromptResult prmRes = acdoc.Editor.GetString(prmtStr);

            userInput =Convert.ToDouble(prmRes.StringResult);

            return userInput;
        }

        private static void CreateLayer(string layerName)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;

            Database dBase = doc.Database;

            using (Transaction acTrans = doc.TransactionManager.StartTransaction())
            {
                LayerTable aclayerTb;
                aclayerTb = acTrans.GetObject(dBase.LayerTableId, OpenMode.ForRead ) as LayerTable;

                if (aclayerTb.Has(layerName) != true)
                {

                    LayerTable acLayerTb;
                    acLayerTb = acTrans.GetObject(dBase.LayerTableId, OpenMode.ForWrite) as LayerTable;

                    LayerTableRecord acLayerTbRec;
                    acLayerTbRec = acTrans.GetObject(acLayerTb["0"], OpenMode.ForRead).Clone() as LayerTableRecord;

                    acLayerTbRec.Name = layerName;

                    acLayerTb.Add(acLayerTbRec);

                    acTrans.AddNewlyCreatedDBObject(acLayerTbRec, true);

                    acTrans.Commit();

                }
               

            }

        }

    }
}
