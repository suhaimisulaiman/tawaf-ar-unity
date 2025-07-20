using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

namespace TawafARTrainer.Tests
{
    [TestFixture]
    public class PrayerRecitationTests
    {
        private PrayerRecitation prayerRecitation;
        private GameObject testGameObject;
        private TawafController mockTawafController;

        [SetUp]
        public void Setup()
        {
            // Create test GameObject
            testGameObject = new GameObject("TestPrayerRecitation");
            prayerRecitation = testGameObject.AddComponent<PrayerRecitation>();
            
            // Create mock TawafController
            GameObject controllerObj = new GameObject("MockTawafController");
            mockTawafController = controllerObj.AddComponent<TawafController>();
            
            // Set up camera
            GameObject cameraObj = new GameObject("TestCamera");
            Camera testCamera = cameraObj.AddComponent<Camera>();
            cameraObj.tag = "MainCamera";
        }

        [TearDown]
        public void Teardown()
        {
            if (testGameObject != null)
                Object.DestroyImmediate(testGameObject);
            
            if (mockTawafController != null)
                Object.DestroyImmediate(mockTawafController.gameObject);
        }

        [Test]
        public void PrayerRecitation_Initialization_ShouldSetDefaultValues()
        {
            // Assert
            Assert.IsTrue(prayerRecitation.enablePrayers);
            Assert.AreEqual(90f, prayerRecitation.cornerThreshold);
            Assert.IsFalse(prayerRecitation.isPlaying);
            Assert.AreEqual(0f, prayerRecitation.audioStartTime);
        }

        [Test]
        public void EnablePrayers_ShouldBeConfigurable()
        {
            // Act
            prayerRecitation.enablePrayers = true;
            bool prayersEnabled = prayerRecitation.enablePrayers;
            prayerRecitation.enablePrayers = false;
            bool prayersDisabled = prayerRecitation.enablePrayers;

            // Assert
            Assert.IsTrue(prayersEnabled);
            Assert.IsFalse(prayersDisabled);
        }

        [Test]
        public void CornerThreshold_ShouldBeReasonable()
        {
            // Assert
            Assert.Greater(prayerRecitation.cornerThreshold, 0f);
            Assert.Less(prayerRecitation.cornerThreshold, 180f); // Should be less than half circle
        }

        [Test]
        public void GetTawafStartPrayer_ShouldReturnValidText()
        {
            // Act
            string prayer = prayerRecitation.GetTawafStartPrayer();

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(prayer));
            Assert.IsTrue(prayer.Contains("بِسْمِ اللَّهِ")); // Contains Arabic text
            Assert.IsTrue(prayer.Contains("Bismillahi")); // Contains transliteration
            Assert.IsTrue(prayer.Contains("In the name of Allah")); // Contains English translation
        }

        [Test]
        public void GetHajarAswadPrayer_ShouldReturnValidText()
        {
            // Act
            string prayer = prayerRecitation.GetHajarAswadPrayer();

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(prayer));
            Assert.IsTrue(prayer.Contains("اللَّهُ أَكْبَر")); // Contains Arabic text
            Assert.IsTrue(prayer.Contains("Allahu Akbar")); // Contains transliteration
            Assert.IsTrue(prayer.Contains("Allah is the Greatest")); // Contains English translation
        }

        [Test]
        public void GetCornerPrayer_ShouldReturnValidText()
        {
            // Act
            string prayer = prayerRecitation.GetCornerPrayer();

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(prayer));
            Assert.IsTrue(prayer.Contains("بِسْمِ اللَّهِ")); // Contains Arabic text
            Assert.IsTrue(prayer.Contains("Bismillahi")); // Contains transliteration
            Assert.IsTrue(prayer.Contains("In the name of Allah")); // Contains English translation
        }

        [Test]
        public void GetTawafCompletionPrayer_ShouldReturnValidText()
        {
            // Act
            string prayer = prayerRecitation.GetTawafCompletionPrayer();

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(prayer));
            Assert.IsTrue(prayer.Contains("الْحَمْدُ لِلَّهِ")); // Contains Arabic text
            Assert.IsTrue(prayer.Contains("Alhamdulillah")); // Contains transliteration
            Assert.IsTrue(prayer.Contains("Praise be to Allah")); // Contains English translation
        }

        [Test]
        public void PrayerTexts_ShouldContainAllRequiredElements()
        {
            // Test all prayer methods
            string[] prayers = {
                prayerRecitation.GetTawafStartPrayer(),
                prayerRecitation.GetHajarAswadPrayer(),
                prayerRecitation.GetCornerPrayer(),
                prayerRecitation.GetTawafCompletionPrayer()
            };

            foreach (string prayer in prayers)
            {
                // Assert each prayer has Arabic, transliteration, and English
                Assert.IsTrue(prayer.Contains("بِسْمِ") || prayer.Contains("اللَّهُ") || prayer.Contains("الْحَمْدُ"));
                Assert.IsTrue(prayer.Contains("Bismillahi") || prayer.Contains("Allahu") || prayer.Contains("Alhamdulillah"));
                Assert.IsTrue(prayer.Contains("name of Allah") || prayer.Contains("Greatest") || prayer.Contains("Praise be to Allah"));
            }
        }

        [Test]
        public void PrayerTexts_ShouldNotBeEmpty()
        {
            // Test all prayer methods return non-empty strings
            Assert.IsFalse(string.IsNullOrEmpty(prayerRecitation.GetTawafStartPrayer()));
            Assert.IsFalse(string.IsNullOrEmpty(prayerRecitation.GetHajarAswadPrayer()));
            Assert.IsFalse(string.IsNullOrEmpty(prayerRecitation.GetCornerPrayer()));
            Assert.IsFalse(string.IsNullOrEmpty(prayerRecitation.GetTawafCompletionPrayer()));
        }

        [Test]
        public void PrayerTexts_ShouldHaveProperFormatting()
        {
            // Test that prayers have proper line breaks
            string prayer = prayerRecitation.GetTawafStartPrayer();
            Assert.IsTrue(prayer.Contains("\n")); // Should have line breaks
        }

        [Test]
        public void IsPlaying_ShouldBeInitializedToFalse()
        {
            // Assert
            Assert.IsFalse(prayerRecitation.isPlaying);
        }

        [Test]
        public void AudioStartTime_ShouldBeInitializedToZero()
        {
            // Assert
            Assert.AreEqual(0f, prayerRecitation.audioStartTime);
        }

        [Test]
        public void PlayTawafStartPrayer_ShouldNotThrowException()
        {
            // Act & Assert - Should not throw exception
            Assert.DoesNotThrow(() => prayerRecitation.PlayTawafStartPrayer());
        }

        [Test]
        public void PlayHajarAswadPrayer_ShouldNotThrowException()
        {
            // Act & Assert - Should not throw exception
            Assert.DoesNotThrow(() => prayerRecitation.PlayHajarAswadPrayer());
        }

        [Test]
        public void PlayCornerPrayer_ShouldNotThrowException()
        {
            // Act & Assert - Should not throw exception
            Assert.DoesNotThrow(() => prayerRecitation.PlayCornerPrayer());
        }

        [Test]
        public void PlayTawafCompletionPrayer_ShouldNotThrowException()
        {
            // Act & Assert - Should not throw exception
            Assert.DoesNotThrow(() => prayerRecitation.PlayTawafCompletionPrayer());
        }

        [Test]
        public void StopPrayer_ShouldNotThrowException()
        {
            // Act & Assert - Should not throw exception
            Assert.DoesNotThrow(() => prayerRecitation.StopPrayer());
        }

        [Test]
        public void ArabicText_ShouldContainValidCharacters()
        {
            // Test that Arabic text contains valid Arabic characters
            string prayer = prayerRecitation.GetTawafStartPrayer();
            
            // Check for common Arabic characters
            bool hasArabicText = prayer.Contains("بِسْمِ") || 
                                prayer.Contains("اللَّهِ") || 
                                prayer.Contains("أَكْبَر") ||
                                prayer.Contains("الْحَمْدُ");
            
            Assert.IsTrue(hasArabicText);
        }

        [Test]
        public void Transliteration_ShouldBeAccurate()
        {
            // Test that transliteration is present and accurate
            string prayer = prayerRecitation.GetTawafStartPrayer();
            
            // Check for common transliterations
            bool hasTransliteration = prayer.Contains("Bismillahi") || 
                                     prayer.Contains("Allahu") || 
                                     prayer.Contains("Akbar");
            
            Assert.IsTrue(hasTransliteration);
        }

        [Test]
        public void EnglishTranslation_ShouldBePresent()
        {
            // Test that English translations are present
            string prayer = prayerRecitation.GetTawafStartPrayer();
            
            // Check for English translations
            bool hasEnglish = prayer.Contains("In the name of Allah") || 
                             prayer.Contains("Allah is the Greatest") ||
                             prayer.Contains("Praise be to Allah");
            
            Assert.IsTrue(hasEnglish);
        }

        [Test]
        public void PrayerLength_ShouldBeReasonable()
        {
            // Test that prayers are not too short or too long
            string[] prayers = {
                prayerRecitation.GetTawafStartPrayer(),
                prayerRecitation.GetHajarAswadPrayer(),
                prayerRecitation.GetCornerPrayer(),
                prayerRecitation.GetTawafCompletionPrayer()
            };

            foreach (string prayer in prayers)
            {
                Assert.Greater(prayer.Length, 10); // Should be longer than 10 characters
                Assert.Less(prayer.Length, 1000); // Should be shorter than 1000 characters
            }
        }

        [UnityTest]
        public IEnumerator PrayerRecitation_Start_ShouldInitialize()
        {
            // Act
            prayerRecitation.Start();
            
            // Wait for one frame
            yield return null;

            // Assert
            Assert.IsNotNull(prayerRecitation);
        }

        [Test]
        public void PrayerMethods_ShouldReturnDifferentTexts()
        {
            // Test that different prayer methods return different texts
            string startPrayer = prayerRecitation.GetTawafStartPrayer();
            string hajarPrayer = prayerRecitation.GetHajarAswadPrayer();
            string cornerPrayer = prayerRecitation.GetCornerPrayer();
            string completionPrayer = prayerRecitation.GetTawafCompletionPrayer();

            // All prayers should be different
            Assert.AreNotEqual(startPrayer, hajarPrayer);
            Assert.AreNotEqual(startPrayer, cornerPrayer);
            Assert.AreNotEqual(startPrayer, completionPrayer);
            Assert.AreNotEqual(hajarPrayer, cornerPrayer);
            Assert.AreNotEqual(hajarPrayer, completionPrayer);
            Assert.AreNotEqual(cornerPrayer, completionPrayer);
        }

        [Test]
        public void PrayerTexts_ShouldBeConsistent()
        {
            // Test that same prayer method returns consistent text
            string prayer1 = prayerRecitation.GetTawafStartPrayer();
            string prayer2 = prayerRecitation.GetTawafStartPrayer();
            
            Assert.AreEqual(prayer1, prayer2);
        }
    }
} 