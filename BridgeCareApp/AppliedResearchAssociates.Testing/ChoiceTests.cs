using Newtonsoft.Json;
using NUnit.Framework;

namespace AppliedResearchAssociates.Testing
{
    public class ChoiceTests
    {
        [Test]
        public void NewtonsoftJsonSerializationRoundtripAsMember()
        {
            var subject = new ChoiceSerializationTestSubject { Member = 123 };
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            var json = JsonConvert.SerializeObject(subject, settings);
            var subjectAgain = JsonConvert.DeserializeObject<ChoiceSerializationTestSubject>(json, settings);
            Assert.That(subjectAgain, Is.EqualTo(subject).Using<ChoiceSerializationTestSubject>(SelectionEqualityComparer<ChoiceSerializationTestSubject>.Create(_ => _.Member)));
        }

        [Test]
        public void NewtonsoftJsonSerializationRoundtripAsRoot()
        {
            var subject = Choice<int, string>.Of(123);
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            var json = JsonConvert.SerializeObject(subject, typeof(Choice<int, string>), settings);
            var subjectAgain = JsonConvert.DeserializeObject<Choice<int, string>>(json, settings);
            Assert.That(subjectAgain, Is.EqualTo(subject));
        }
    }
}
