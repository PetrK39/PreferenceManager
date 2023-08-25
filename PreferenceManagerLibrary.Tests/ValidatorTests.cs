using PreferenceManagerLibrary.Preferences;
using PreferenceManagerLibrary.Validation;
using System.Net.Http.Headers;
using System.Runtime.Serialization;

namespace PreferenceManagerLibrary.Tests
{
    public class ValidatorTests_NumberValidator
    {
        [Test]
        public void Validate_Basic_Valid()
        {
            var dv = new NumberValidator<double>();
            var iv = new NumberValidator<int>();
            var bv = new NumberValidator<byte>();

            var v = new[] {
                dv.ValidateBool("123,123"),
                dv.ValidateBool("123,123 "),
                dv.ValidateBool(" 123,123"),

                iv.ValidateBool("-123"),

                bv.ValidateBool("123")
            };

            Assert.That(v, Is.All.True);
        }

        [Test]
        public void Validate_Basic_NonValid()
        {
            var dv = new NumberValidator<double>();
            var iv = new NumberValidator<int>();
            var bv = new NumberValidator<byte>();

            var v = new[] {
                dv.ValidateBool(null),
                dv.ValidateBool(""),
                dv.ValidateBool(" "),
                dv.ValidateBool("One"),

                dv.ValidateBool("123.123"),

                iv.ValidateBool("123,123"),

                bv.ValidateBool("1234")
            };

            Assert.That(v, Is.All.False);
        }

        [Test]
        public void Validate_LessGreater_Valid()
        {
            var dvlg = new NumberValidator<double>().AddGreaterThan(-100).AddLessThan(100);
            var dvlege = new NumberValidator<double>().AddGreaterOrEqualsThan(-100).AddLessOrEqualsThan(100);

            var v = new[] {
                dvlg.ValidateBool("99,9"),
                dvlg.ValidateBool("-99,9"),

                dvlege.ValidateBool("100"),
                dvlege.ValidateBool("-100")
            };

            Assert.That(v, Is.All.True);
        }

        [Test]
        public void Validate_LessGreater_NonValid()
        {
            var dvlg = new NumberValidator<double>().AddLessThan(100).AddGreaterThan(-100);
            var dvlege = new NumberValidator<double>().AddLessOrEqualsThan(100).AddGreaterOrEqualsThan(-100);

            var v = new[] {
                dvlg.ValidateBool("100,0"),
                dvlg.ValidateBool("-100,0"),

                dvlege.ValidateBool("100,1"),
                dvlege.ValidateBool("-100,1")
            };

            Assert.That(v, Is.All.False);
        }

        [Test]
        public void Validate_Equals_Valid()
        {
            var dve = new NumberValidator<double>().AddEqualsTo(values: new[] { -1.25, 1, 1.25 });
            var dvne = new NumberValidator<double>().AddNotEqualsTo(values: new[] { -1.25, 1, 1.25 });

            var v = new[] {
                dve.ValidateBool("-1,25"),
                dve.ValidateBool("1"),
                dve.ValidateBool("1,25"),

                dvne.ValidateBool("-1,5"),
                dvne.ValidateBool("0"),
                dvne.ValidateBool("1,5")
            };

            Assert.That(v, Is.All.True);
        }

        [Test]
        public void Validate_Equals_NonValid()
        {
            var dve = new NumberValidator<double>().AddEqualsTo(values: new[] { -1.25, 1, 1.25 });
            var dvne = new NumberValidator<double>().AddNotEqualsTo(values: new[] { -1.25, 1, 1.25 });

            var v = new[] {
                dve.ValidateBool("-1,5"),
                dve.ValidateBool("0"),
                dve.ValidateBool("1,5"),

                dvne.ValidateBool("-1,25"),
                dvne.ValidateBool("1"),
                dvne.ValidateBool("1,25")
            };

            Assert.That(v, Is.All.False);
        }

        [Test]
        public void Validate_Rejects_Valid()
        {
            var dvrd = new NumberValidator<double>().SetRejectDecimal();
            var dvri = new NumberValidator<double>().SetRejectInfinity();
            var dvrn = new NumberValidator<double>().SetRejectNaN();

            var v = new[] {
                dvrd.ValidateBool("0"),

                dvri.ValidateBool("0"),

                dvrn.ValidateBool("0")
            };

            Assert.That(v, Is.All.True);
        }

        [Test]
        public void Validate_Rejects_NonValid()
        {
            var dvrd = new NumberValidator<double>().SetRejectDecimal();
            var dvri = new NumberValidator<double>().SetRejectInfinity();
            var dvrn = new NumberValidator<double>().SetRejectNaN();

            var v = new[] {
                dvrd.ValidateBool("0,1"),

                dvri.ValidateBool("Infinity"),

                dvrn.ValidateBool("NaN")
            };

            Assert.That(v, Is.All.False);
        }

        [Test]
        public void SetReject_NonValidType_ThrowsInvalidOperationException()
        {
            Assert.Catch<InvalidOperationException>(() => new NumberValidator<int>().SetRejectDecimal());
            Assert.Catch<InvalidOperationException>(() => new NumberValidator<byte>().SetRejectInfinity());
            Assert.Catch<InvalidOperationException>(() => new NumberValidator<long>().SetRejectNaN());
        }

        [Test]
        public void Validate_Restrictions_DataErrorInfo()
        {
            var testFailMsg = "TestFailMessage";
            var testGreatherThanMsg = "TestGreatherThanMessage {0}";

            var sv = new NumberValidator<int>(failMessage: testFailMsg).AddGreaterThan(10, testGreatherThanMsg);

            var err = new[]
            {
                sv.ValidateErrorInfo("non int") == testFailMsg,
                sv.ValidateErrorInfo("0") == string.Format(testGreatherThanMsg, 10)
            };

            Assert.That(err, Is.All.True);
        }
    }
    public class ValidatorTests_StringValidator
    {
        [Test]
        public void Validate_Basic_Valid()
        {
            var dv = new StringValidator();

            var v = new[] {
                dv.ValidateBool("123,123"),
                dv.ValidateBool("Test"),
            };

            Assert.That(v, Is.All.True);
        }
        [Test]
        public void Validate_Basic_NonValid()
        {
            var dv = new StringValidator();

            var v = new[] {
                dv.ValidateBool(string.Empty),
                dv.ValidateBool(""),
                dv.ValidateBool("   "),
            };

            Assert.That(v, Is.All.False);
        }
        [Test]
        public void Validate_Restrictions_Valid()
        {
            var sv1 = new StringValidator().AddEqualsTo(values: new int[] { 1, 6, 10 });
            var sv2 = new StringValidator().AddNotEqualsTo(values: new int[] { 2, 5, 11 });

            var sv3 = new StringValidator().AddLengthLessThan(7);
            var sv4 = new StringValidator().AddLessOrEqualsThan(6);

            var sv5 = new StringValidator().AddGreaterThan(4);
            var sv6 = new StringValidator().AddGreaterOrEqualsThan(5);

            var sv7 = new StringValidator().SetStartsWith("_");
            var sv8 = new StringValidator().SetEndsWith(".");

            var sv9 = new StringValidator().AddContains("e");
            var sv10 = new StringValidator().AddMatches(".test.");

            var sv11 = new StringValidator().AddCustom((s) => s == "_test." ? string.Empty : "String does not match");

            var testString = "_test.";

            var v = new[]
            {
                sv1.ValidateBool(testString),
                sv2.ValidateBool(testString),

                sv3.ValidateBool(testString),
                sv4.ValidateBool(testString),

                sv5.ValidateBool(testString),
                sv6.ValidateBool(testString),

                sv7.ValidateBool(testString),
                sv8.ValidateBool(testString),

                sv9.ValidateBool(testString),
                sv10.ValidateBool(testString),

                sv11.ValidateBool(testString)
            };

            Assert.That(v, Is.All.True);
        }
        [Test]
        public void Validate_Restrictions_NonValid()
        {
            var sv1 = new StringValidator().AddEqualsTo(values: new int[] { 1, 6, 10 });
            var sv2 = new StringValidator().AddNotEqualsTo(values: new int[] { 2, 5, 11 });

            var sv3 = new StringValidator().AddLengthLessThan(5);
            var sv4 = new StringValidator().AddLessOrEqualsThan(4);

            var sv5 = new StringValidator().AddGreaterThan(5);
            var sv6 = new StringValidator().AddGreaterOrEqualsThan(6);

            var sv7 = new StringValidator().SetStartsWith("_");
            var sv8 = new StringValidator().SetEndsWith(".");

            var sv9 = new StringValidator().AddContains("e");
            var sv10 = new StringValidator().AddMatches(".test.");

            var sv11 = new StringValidator().AddCustom((s) => s == "_test." ? string.Empty : "String does not match");

            var testString = "@tst!";

            var v = new[]
            {
                sv1.ValidateBool(testString),
                sv2.ValidateBool(testString),

                sv3.ValidateBool(testString),
                sv4.ValidateBool(testString),

                sv5.ValidateBool(testString),
                sv6.ValidateBool(testString),

                sv7.ValidateBool(testString),
                sv8.ValidateBool(testString),

                sv9.ValidateBool(testString),
                sv10.ValidateBool(testString),

                sv11.ValidateBool(testString)
            };

            Assert.That(v, Is.All.False);
        }
        [Test]
        public void Validate_Restrictions_DataErrorInfo()
        {
            var testEmptyMsg = "TestEmptyMessage";
            var testGreatherThanMsg = "TestGreatherThanMessage {0}";

            var sv = new StringValidator(emptyMessage: testEmptyMsg).AddGreaterThan(10, testGreatherThanMsg);

            var err = new[]
            {
                sv.ValidateErrorInfo(string.Empty) == testEmptyMsg,
                sv.ValidateErrorInfo("---") == string.Format(testGreatherThanMsg, 10)
            };

            Assert.That(err, Is.All.True);
        }
    }
}
