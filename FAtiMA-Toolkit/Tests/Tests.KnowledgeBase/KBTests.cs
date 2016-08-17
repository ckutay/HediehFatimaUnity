﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GAIPS.Serialization;
using KnowledgeBase;
using NUnit.Framework;
using Utilities;
using WellFormedNames;

namespace Tests.KnowledgeBase
{
	[TestFixture]
	public class KBTests
	{
		[TestCase("true")]
		[TestCase("10")]
		[TestCase("Johan")]
		[TestCase("10e+34")]
		public void Test_Tell_Fail_Primitive_Property(string primitiveName)
		{
			var kb = new KB((Name)"John");
			Assert.Throws<ArgumentException>(() => kb.Tell((Name)primitiveName, Name.BuildName(true)));
		}

		[TestCase("Likes(*)")]
		[TestCase("Has([x])")]
		public void Test_Tell_Fail_NonConstant_Property(string propertyName)
		{
			var kb = new KB((Name)"John");
			Assert.Throws<ArgumentException>(() => kb.Tell((Name)propertyName, Name.BuildName(true)));
		}

		[Test]
		public void Test_Tell_Fail_Add_Self_To_Universal_Context()
		{
			var kb = new KB((Name)"John");
			Assert.Throws<InvalidOperationException>(() => kb.Tell((Name)"Likes(Self)", Name.BuildName(true),Name.UNIVERSAL_SYMBOL));
		}

		[TestCase("[x]", typeof(ArgumentException))]
		[TestCase("John([x])",typeof(ArgumentException))]
		[TestCase("John(Mary,Steve)",typeof(ArgumentException))]
		[TestCase("John(*)", typeof(ArgumentException))]
		[TestCase("*(John)", typeof(ArgumentException))]
		[TestCase("John(*(Steven([x])))", typeof(ArgumentException))]
		[TestCase("John(Mary(Steve(Self)))", typeof(ArgumentException))]
		public void Test_Tell_Fail_Assert_Perspective(string perspective,Type exceptionType)
		{
			var kb = new KB((Name)"John");

			Assert.Throws(exceptionType,() => kb.Tell((Name)"Likes(Mary)", Name.BuildName(true), (Name)perspective));
		}

		[TestCase("ToM(Mary,Likes(Self))", "Mary(Self)")]
		[TestCase("ToM(Mary,Likes(Self),Marty)", "Self")]
		[TestCase("ToM(John,ToM(Mary,Likes(John)))", "Mary")]
		public void Test_Tell_Fail_Property_ToM_Transform(string property,string perspective)
		{
			var kb = new KB((Name)"John");
			Assert.Throws<ArgumentException>(() => kb.Tell((Name) property, Name.BuildName(true),(Name)perspective));
		}

		[TestCase("Count(Tiger)", "Self")]
		[TestCase("Count(Tiger)", "*")]
		[TestCase("Count(Tiger)", "Mary")]
		[TestCase("Count(Tiger)", "Mary(Self)")]
		[TestCase("ToM(Mary,Count(Tiger))", "Self")]
		[TestCase("ToM(Mary,ToM(Self,Count(Tiger)))", "Self")]
		[TestCase("ToM(John,Count(Tiger))", "Mary")]
		public void Test_Tell_Fail_DynamicProperty(string property, string perspective)
		{
			var kb = new KB((Name)"John");
			Assert.Throws<ArgumentException>(() => kb.Tell((Name)property, Name.BuildName(true), (Name)perspective));
		}

		[Test]
		public void Test_Tell_Fail_Complext_Property_With_ToM()
		{
			const string property = "ToM(Mary,ToM(Self,Has(ToM(Mary,Ball))))";
			var kb = new KB((Name)"John");
			Assert.Throws<ArgumentException>(() => kb.Tell((Name)property, Name.BuildName(true)));
		}

		[Test]
		public void Test_Tell_Pass_Basic_Property_With_ToM()
		{
			const string property = "ToM(Mary,ToM(Self,Has(Ball)))";
			var kb = new KB((Name)"John");
			kb.Tell((Name) property, Name.BuildName(true));
		}

		[Test]
		public void Test_AskProperty_Self()
		{
			var kb = new KB((Name)"John");
			var value = Name.BuildName(kb.AskProperty(Name.SELF_SYMBOL).ToString());
			Assert.AreEqual(value, kb.Perspective);
		}

		private class TestFactory
		{
			public static IEnumerable<TestCaseData> Test_Simple_Tell_Valid_Cases()
			{
				yield return new TestCaseData((Name)"like(Ann,john)", true);
				yield return new TestCaseData((Name)"like(Ann,Amy)", true);
				yield return new TestCaseData((Name)"like(Ann,Mike)", true);
				yield return new TestCaseData((Name)"like(Ann,Steven)", true);
				yield return new TestCaseData((Name)"Color(id_2433)", "Blue");
			}

			public static IEnumerable<TestCaseData> Test_OperatorRegist_Cases()
			{
				DynamicPropertyCalculator p = (kb2, pers, args, subs) =>
				{
					return Enumerable.Empty<Pair<Name, SubstitutionSet>>();
				};
				yield return new TestCaseData(PopulatedTestMemory(), (Name)"Count([x])", p, (Name)"Count(IsAlive([x]))", null);
				yield return new TestCaseData(PopulatedTestMemory(), (Name)"Count([x])", p, (Name)"Count([y])", new SubstitutionSet(new Substitution("[y]/IsAlive([x])")));
			}

			public static IEnumerable<TestCaseData> MemoryData()
			{
				yield return new TestCaseData((Name)"Strength(John)", (byte)5);
				yield return new TestCaseData((Name)"Strength(Mary)", (sbyte)3);
				yield return new TestCaseData((Name)"Strength(Leonidas)", (short)500);
				yield return new TestCaseData((Name)"Strength(Goku)", (uint)9001f);
				yield return new TestCaseData((Name)"Strength(SuperMan)", ulong.MaxValue);
				yield return new TestCaseData((Name)"Strength(Saitama)", float.MaxValue);
				yield return new TestCaseData((Name)"Race(Saitama)", "human");
				yield return new TestCaseData((Name)"Race(Superman)", "kriptonian");
				yield return new TestCaseData((Name)"Race(Goku)", "sayian");
				yield return new TestCaseData((Name)"Race(Leonidas)", "human");
				yield return new TestCaseData((Name)"Race(Mary)", "human");
				yield return new TestCaseData((Name)"Race(John)", "human");
				yield return new TestCaseData((Name)"Job(Saitama)", "super-hero");
				yield return new TestCaseData((Name)"Job(Superman)", "super-hero");
				yield return new TestCaseData((Name)"Job(Leonidas)", "Spartan");
				yield return new TestCaseData((Name)"AKA(Saitama)", "One-Punch_Man");
				yield return new TestCaseData((Name)"AKA(Superman)", "Clark_Kent");
				yield return new TestCaseData((Name)"AKA(Goku)", "Kakarot");
				yield return new TestCaseData((Name)"Hobby(Saitama)", "super-hero");
				yield return new TestCaseData((Name)"Hobby(Goku)", "training");
				yield return new TestCaseData((Name)"IsAlive(Leonidas)", false);
				yield return new TestCaseData((Name)"IsAlive(Saitama)", true);
				yield return new TestCaseData((Name)"IsAlive(Superman)", true);
				yield return new TestCaseData((Name)"IsAlive(John)", true);

				yield return new TestCaseData((Name)"Strength(Name(Self))", 7, typeof(Exception));
				yield return new TestCaseData((Name)"Name(Self)", "Titus", typeof(Exception));
				yield return new TestCaseData((Name)"Name(Titus)", "Titus");
				yield return new TestCaseData((Name)"Strength(Name(Self))", 7);
			}

			public static IEnumerable<TestCaseData> Test_Simple_Tell_Invalid_Cases()
			{
				yield return new TestCaseData((Name)"[x]", false, typeof(ArgumentException));
				yield return new TestCaseData((Name)"like(self,[x])", false, typeof(ArgumentException));
				yield return new TestCaseData((Name)"like(self,Color(Ball))", false, typeof(Exception));
				yield return new TestCaseData((Name)"like(self,Color(A(B,D)))", false, typeof(Exception));
				yield return new TestCaseData((Name)"10", 35, typeof(ArgumentException));
				yield return new TestCaseData((Name)"10", 10, typeof(ArgumentException));
				yield return new TestCaseData((Name)"true", 25, typeof(ArgumentException));
				yield return new TestCaseData((Name)"false", true, typeof(ArgumentException));
			}

			//public static int NumOfPersistentEntries()
			//{
			//	return MemoryData().Count(d => 
			//		d.Arguments.Length > 2 && (bool) d.Arguments[2]
			//		);
			//}

			public static KB PopulatedTestMemory()
			{
				KB kb = new KB((Name)"Me");
				foreach (var t in MemoryData())
				{
					Name property = (Name) t.Arguments[0];
					Name value = Name.BuildName(t.Arguments[1]);
					try
					{
						//if(t.Arguments.Length>2)
						//	kb.Tell((Name)t.Arguments[0], PrimitiveValue.Cast(t.Arguments[1]),(bool)t.Arguments[2]);
						//else
						kb.Tell(property, value);
					}
					catch (Exception e)
					{
						if (t.Arguments.Length > 2)
							Assert.AreEqual(e.GetType(), t.Arguments[2]);
						else
							Assert.Fail($"An exception was thrown unexpectedly while evaluating {property} = {value}: {e}");
					}
				}
				return kb;
			}
		}

		[TestCaseSource(typeof(TestFactory), nameof(TestFactory.Test_Simple_Tell_Valid_Cases))]
		public void Test_Simple_Tell_Valid(Name name, object value)
		{
			var kb = new KB((Name)"Me");
			kb.Tell(name, Name.BuildName(value));
		}

		[TestCaseSource(typeof(TestFactory), nameof(TestFactory.Test_Simple_Tell_Invalid_Cases))]
		public void Test_Simple_Tell_Invalid(Name name, object value, Type expectedException)
		{
			var kb = new KB((Name)"Me");
			Assert.Throws(expectedException, () => kb.Tell(name, Name.BuildName(value)));
		}

		[Test]
		public void Test_Acculm_Tell_Valid()
		{
			TestFactory.PopulatedTestMemory();
		}

		//[Test]
		//public void Test_RemoveProperty()
		//{
		//	throw new NotImplementedException();
		//}

		[TestCase("35", 35)]
		[TestCase("-9223372036854775807", -9223372036854775807)]
		[TestCase("-9.43", -9.43)]
		[TestCase("-9.43e-1", -9.43e-1)]
		[TestCase("true", true)]
		[TestCase("FALSE", false)]
		[TestCase("3.40282347E+39", 3.40282347E+39)]
		public void Test_PrimitiveValuesAsk(string str, object expect)
		{
			Name v = (Name)str;
			KB kb = new KB((Name)"Me");
			var value = kb.AskProperty(v);
			Assert.NotNull(value);

			Assert.AreEqual(value, Name.BuildName(expect));
		}

		[Test]
		public void Test_OperatorRegist_Fail_Duplicate()
		{
			var kb = new KB((Name)"Me");
			Assert.Throws<ArgumentException>(
				() => kb.RegistDynamicProperty((Name)"Count([y])", ((kb1, pers, args, constraints) => null)));
		}

		[Test]
		public void Test_OperatorRegist_Fail_Same_Template()
		{
			var kb = new KB((Name)"Me");
			Assert.Throws<ArgumentException>(() => kb.RegistDynamicProperty((Name)"Count([x])", ((kb1, p, args, constraints) => null)));
		}

		[Test]
		public void Test_OperatorRegist_Fail_GroundedTemplate()
		{
			var kb = new KB((Name)"Me");
			Assert.Throws<ArgumentException>(() => kb.RegistDynamicProperty((Name)"Count(John)", ((kb1, p, args, constraints) => null)));
		}

		[Test]
		public void Test_OperatorRegist_Fail_Null_Surogate()
		{
			var kb = new KB((Name)"Me");
			Assert.Throws<ArgumentNullException>(() => kb.RegistDynamicProperty((Name)"Count(John)", null));
		}

		[Test]
		public void Test_OperatorRegist_Fail_ConstantProperties()
		{
			var kb = new KB((Name)"Me");
			Assert.Throws<ArgumentException>(() =>
			{
				kb.Tell((Name)"Count(John)", Name.BuildName(3));
				kb.RegistDynamicProperty((Name)"Count([x])", ((kb1, p, args, constraints) => null));
			});
		}

		[Test]
		public void Test_Tell_Fail_OperatorRegist()
		{
			var kb = new KB((Name)"Me");
			Assert.Throws<ArgumentException>(() =>
			{
				kb.RegistDynamicProperty((Name)"Count([x])", ((kb1, p, args, constraints) => null));
				kb.Tell((Name)"Count(John)", Name.BuildName(3));
			});
		}

		[Test]
		public void Test_Tell_Fail_Add_Self_To_Universal()
		{
			var kb = new KB(Name.BuildName("Matt"));
			Assert.Throws<InvalidOperationException>(() => { kb.Tell((Name)"IsPerson(Self)", Name.BuildName(true), Name.UNIVERSAL_SYMBOL); });
		}

		[TestCase("Matt", "IsPerson(Matt)", "*", "IsPerson(Matt)", "Self")]
		[TestCase("Matt", "IsPerson(Matt)", "*", "IsPerson(Matt)", "Mary")]
		[TestCase("Matt", "IsPerson(Self)", "Self", "IsPerson(Self)", "Self")]
		[TestCase("Matt", "IsPerson(Matt)", "Self", "IsPerson(Self)", "Matt")]
		[TestCase("Matt", "IsPerson(Self)", "Mary", "IsPerson(Mary)", "Mary")]
		[TestCase("Matt", "IsPerson(Self)", "Matt", "IsPerson(Self)", "Matt")]
		public void Test_Tell_Pass_Add_With_Perspective(string nativePerspective, string tellPerdicate, string tellPerspective, string queryPerdicate, string queryPerspective)
		{
			var kb = new KB(Name.BuildName(nativePerspective));
			kb.Tell(Name.BuildName(tellPerdicate), Name.BuildName(true), Name.BuildName(tellPerspective));

			using (var stream = new MemoryStream())
			{
				var formater = new JSONSerializer();
				formater.Serialize(stream, kb);
				stream.Seek(0, SeekOrigin.Begin);
				Console.WriteLine(new StreamReader(stream).ReadToEnd());
			}

			var r = kb.AskProperty(Name.BuildName(queryPerdicate), Name.BuildName(queryPerspective));
			bool b;
			if (!r.TryConvertToValue(out b))
				Assert.Fail();

			Assert.IsTrue(b);
		}

		[Test]
		public void Test_Fail_Tell_With_Nil_Perspective()
		{
			var kb = new KB(Name.BuildName("Mark"));
			Assert.Throws<ArgumentException>(() => kb.Tell(Name.BuildName("IsPerson(Self)"), Name.BuildName(true), Name.NIL_SYMBOL));
		}

		[Test]
		public void Test_Tell_Pass_Add_Self_Belief_and_Change_Perspective_01()
		{
			var kb = new KB(Name.BuildName("Mark"));
			kb.Tell(Name.BuildName("IsPerson(Self)"), Name.BuildName(true));

			kb.SetPerspective(Name.BuildName("Mary"));

			Assert.Null(kb.AskProperty(Name.BuildName("IsPerson(Mark)")));

			var n = kb.AskProperty(Name.BuildName("IsPerson(Mary)"));
			bool b;
			if(!n.TryConvertToValue(out b))
				Assert.Fail();
			Assert.True(b);
		}

		[Test]
		public void Test_Tell_Pass_Add_Self_Belief_and_Change_Perspective_02()
		{
			var kb = new KB(Name.BuildName("Mark"));
			kb.Tell(Name.BuildName("IsPerson(Self)"), Name.BuildName(true),Name.BuildName("John(Self)"));

			kb.SetPerspective(Name.BuildName("Mary"));

			Assert.Null(kb.AskProperty(Name.BuildName("IsPerson(Mark)"), Name.BuildName("John(Self)")));

			var n = kb.AskProperty(Name.BuildName("IsPerson(Mary)"), Name.BuildName("John(Self)"));
			bool b;
			if(!n.TryConvertToValue(out b))
				Assert.Fail();
			Assert.True(b);
		}

		[Test]
		public void Test_Fail_Change_Perspective_Conflict()
		{
			var kb = new KB(Name.BuildName("Mark"));
			kb.Tell(Name.BuildName("IsPerson(Self)"), Name.BuildName(true), Name.BuildName("John(Self)"));

			Assert.Throws<ArgumentException>(()=> kb.SetPerspective(Name.BuildName("John")));
		}

		[TestCase("*")]
		[TestCase("-")]
		[TestCase("Test(Mark)")]
		[TestCase("Self")]
		public void Test_Fail_Change_Perspective_To_Invalid_Perspective(string perspective)
		{
			var kb = new KB(Name.BuildName("Mark"));
			kb.Tell(Name.BuildName("IsPerson(Self)"), Name.BuildName(true), Name.BuildName("John(Self)"));

			Assert.Throws<ArgumentException>(() => kb.SetPerspective(Name.BuildName(perspective)));
		}
	}
}