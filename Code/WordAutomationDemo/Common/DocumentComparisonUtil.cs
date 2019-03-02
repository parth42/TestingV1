﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aspose.Words;
using Aspose.Words.Layout;

namespace WordAutomationDemo.Common
{
    /// <summary>
    /// Class to compare two MS Word documents
    /// </summary>
    public class DocumentComparisonUtil
    {
        /// <summary>
        /// Compare the two documents using Aspose.Words and save the result as a Word document
        /// </summary>
        /// <param name="document1">First document</param>
        /// <param name="document2">Second document</param>
        /// <param name="comparisonDocument">Comparison document</param>
        public void Compare(string document1, string document2, string comparisonDocument, ref int added, ref int deleted)
        {
            added = 0;
            deleted = 0;

            // Load both documents in Aspose.Words
            Document doc1 = new Document(document1);
            Document doc2 = new Document(document2);
            Document docComp = new Document(document1);
            DocumentBuilder builder = new DocumentBuilder(docComp);

            doc1.Compare(doc2, "a", DateTime.Now);

            foreach (Revision revision in doc1.Revisions)
            {
                switch (revision.RevisionType)
                {
                    case RevisionType.Insertion:
                        added++;
                        break;
                    case RevisionType.Deletion:
                        deleted++;
                        break;
                }
                //Console.WriteLine(revision.RevisionType + ": " + revision.ParentNode);
            }

            doc1.LayoutOptions.RevisionOptions.InsertedTextColor = RevisionColor.Blue;
            doc1.LayoutOptions.RevisionOptions.DeletedTextColor = RevisionColor.Black;
            doc1.LayoutOptions.RevisionOptions.DeletedTextEffect = RevisionTextEffect.StrikeThrough;
            //Debug.WriteLine("Revisions: " + doc1.Revisions.Count);
            doc1.Save(comparisonDocument,SaveFormat.Pdf);
        }
    }
}