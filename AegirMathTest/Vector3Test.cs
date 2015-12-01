﻿using AegirType;
using AegirMathTests.Util;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AegirMathTests
{
    class Vector3Test
    {
        [Test]
        public void DistanceSquared()
        {
            var v1 = new Vector3(0.1f, 100.0f, -5.5f);
            var v2 = new Vector3(1.1f, -2.0f, 5.5f);
            var d = Vector3.DistanceSquared(v1, v2);
            var expectedResult = 10526f;
            Assert.AreEqual(expectedResult, d);
        }

        [Test]
        public void Normalize()
        {
            Vector3 v1 = new Vector3(-10.5f, 0.2f, 1000.0f);
            Vector3 v2 = new Vector3(-10.5f, 0.2f, 1000.0f);
            v1.Normalize();
            var expectedResult = new Vector3(-0.0104994215f, 0.000199988979f, 0.999944866f);
            Assert.That(expectedResult, Is.EqualTo(v1).Using(Vector3Comparer.Epsilon));
            v2 = Vector3.Normalize(v2);
            Assert.That(expectedResult, Is.EqualTo(v2).Using(Vector3Comparer.Epsilon));
        }

        [Test]
        public void Transform()
        {
            // STANDARD OVERLOADS TEST

            var expectedResult1 = new Vector3(51, 58, 65);
            var expectedResult2 = new Vector3(33, -14, -1);

            var v1 = new Vector3(1, 2, 3);
            var m1 = new Matrix(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);

            var v2 = new Vector3(1, 2, 3);
            var q1 = new Quaternion(2, 3, 4, 5);

            Vector3 result1;
            Vector3 result2;

            Assert.That(expectedResult1, Is.EqualTo(Vector3.Transform(v1, m1)).Using(Vector3Comparer.Epsilon));
            Assert.That(expectedResult2, Is.EqualTo(Vector3.Transform(v2, q1)).Using(Vector3Comparer.Epsilon));

            // OUTPUT OVERLOADS TEST

            Vector3.Transform(ref v1, ref m1, out result1);
            Vector3.Transform(ref v2, ref q1, out result2);

            Assert.That(expectedResult1, Is.EqualTo(result1).Using(Vector3Comparer.Epsilon));
            Assert.That(expectedResult2, Is.EqualTo(result2).Using(Vector3Comparer.Epsilon));
        }
    }
}
