using com.jonthysell.Chordious.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Chordious.Web.Controllers
{
    /// <summary>
    /// The home controller.
    /// </summary>
    public class HomeController : Controller
    {
        #region Constants

        private const string xmlPath = "~/Config/builtin.xml";

        #endregion Constants

        #region Variables

        public ConfigFile config;
        public ChordFinderOptions options = new ChordFinderOptions();

        #endregion Variables

        #region Actions

        /// <summary>
        /// Action result for Index.
        /// </summary>
        /// <returns>The index view.</returns>
        public ActionResult Index()
        {
            InitConfig();
            ViewBag.Instruments = GetInstrumentsForDropdown();
            ViewBag.ChordQualities = GetChordQualitiesForDropdown();
            ViewBag.Notes = GetNotesForDropdown();
            return View(options);
        }

        /// <summary>
        /// Action result for About.
        /// </summary>
        /// <returns>The About view.</returns>
        public ActionResult About()
        {
            return View();
        }

        /// <summary>
        /// Action result for How To Use Chordious Web.
        /// </summary>
        /// <returns>The "How To" view.</returns>
        public ActionResult HowTo()
        {
            return View();
        }

        /// <summary>
        /// Action result for the FAQ.
        /// </summary>
        /// <returns>The FAQ view</returns>
        public ActionResult FAQ()
        {
            return View();
        }

        /// <summary>
        /// Action result for contact.
        /// </summary>
        /// <returns>The Contact view.</returns>
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        /// <summary>
        /// Action result for strumming patterns.
        /// </summary>
        /// <returns>The Strumming View.</returns>
        public ActionResult Strumming()
        {
            return View();
        }

        /// <summary>
        /// Action result for chord families.
        /// </summary>
        /// <returns>The Chord Families View.</returns>
        public ActionResult ChordFamilies()
        {
            return View();
        }

        /// <summary>
        /// Action result for links.
        /// </summary>
        /// <returns>The Links View.</returns>
        public ActionResult Links()
        {
            return View();
        }

        #endregion Actions

        #region JQuery Calls

        /// <summary>
        /// Call from JQuery for a list of tunings for the selected instrument.
        /// </summary>
        /// <param name="instrument">The instrument name.</param>
        /// <returns>List of tunings to display in the UI.</returns>
        public JsonResult GetTuningsForDropdown(string instrument)
        {
            InitConfig();

            var tunings = new List<string>();

            var myInstrument = (from t in config.Instruments
                                where t.Value.Name.ToUpper() == instrument.ToUpper()
                                select t.Value).FirstOrDefault();

            if (myInstrument != null)
            {
                foreach (var tuning in myInstrument.Tunings)
                {
                    tunings.Add(tuning.LongName);
                }
            }

            // Add JsonRequest behavior to allow retrieving tunings over http get
            return Json(tunings, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Call from JQuery to find the chords.
        /// </summary>
        /// <param name="instrument">The instrument.</param>
        /// <param name="tuning">The tuning.</param>
        /// <param name="notes">The root note for the chord.</param>
        /// <param name="chordQualities">The chord qualities.</param>
        /// <param name="numFrets">The number of frets to display.</param>
        /// <param name="maxFrets">The maximum number of frets to use.</param>
        /// <param name="maxReach">The maximum fret reach for the fingers.</param>
        /// <param name="autoAddBarres">Auto add barres?</param>
        /// <param name="allowOpenStrings">Allow open strings?</param>
        /// <param name="allowMutedStrings">Allow muted strings?</param>
        /// <param name="mirrorResults">Mirror the results for left-handed chords?</param>
        /// <param name="allowRootlessChords">Allow rootless chords?</param>
        /// <returns>List of SVG images to display in the UI.</returns>
        public JsonResult FindChords(string instrument, string tuning, string notes, string chordQualities, string numFrets, string maxFrets, string maxReach, string autoAddBarres, string allowOpenStrings, string allowMutedStrings, string mirrorResults, string allowRootlessChords)
        {
            // Initialize the ConfigFile object.
            InitConfig();

            // Define the objects.
            Instrument Instrument = null;
            ChordQuality ChordQuality = null;
            ChordFinderOptions myOptions = null;
            ChordFinder chordFinder = null;
            ChordResultSet chordResultSet = null;
            ChordOptions chordOptions = null;
            Tuning Tuning = null;
            Note myNote = NoteUtils.ParseNote(notes);

            Instrument = GetAnInstrument(instrument);
            ChordQuality = GetChordQuality(chordQualities);

            if (Instrument != null)
            {
                // Instantiate the selected tuning object from the instrument.
                Tuning = GetTheTuning(Instrument, tuning);

                // Instantiate the chord finder options.
                myOptions = BuildChordFinderOptions(instrument, tuning, numFrets, maxFrets, maxReach, autoAddBarres, allowOpenStrings, allowMutedStrings, mirrorResults, allowRootlessChords);

                // Instantiate the chord finder object.
                chordFinder = new ChordFinder(Instrument, Tuning);

                // Instantiate the chord result set.
                chordResultSet = chordFinder.FindChords(myNote, ChordQuality, myOptions);

                // Instantiate the chord options.
                chordOptions = BuildChordOptions();

                // Build the list of SVG images to return to the screen.
                return Json(BuildSVGList(chordResultSet, chordOptions), JsonRequestBehavior.AllowGet);
            }
            else
            {
                // The instrument doesn't exist.
                return Json(String.Empty, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion JQuery Calls

        #region Private Methods

        /// <summary>
        /// Initialize the configuration.
        /// </summary>
        private void InitConfig()
        {
            string filePath = Server.MapPath(Url.Content(xmlPath));
            config = new ConfigFile(filePath);
        }

        /// <summary>
        /// Gets a list of notes to display in the dropdown on the UI.
        /// </summary>
        /// <returns>List of notes.</returns>
        private List<SelectListItem> GetNotesForDropdown()
        {
            List<string> notes = new List<string>();

            notes.Add("C");
            notes.Add("C#");
            notes.Add("Db");
            notes.Add("D");
            notes.Add("D#");
            notes.Add("Eb");
            notes.Add("E");
            notes.Add("F");
            notes.Add("F#");
            notes.Add("Gb");
            notes.Add("G");
            notes.Add("G#");
            notes.Add("Ab");
            notes.Add("A");
            notes.Add("A#");
            notes.Add("Bb");
            notes.Add("B");

            List<SelectListItem> notesList = new List<SelectListItem>();

            foreach (string note in notes)
            {
                SelectListItem myNote = new SelectListItem();
                myNote.Text = myNote.Value = note;

                if (note == "C")
                {
                    myNote.Selected = true;
                }

                notesList.Add(myNote);
            }

            return notesList;
        }

        /// <summary>
        /// Gets a list of instruments to display in the dropdown on the UI.
        /// </summary>
        /// <returns>List of instruments.</returns>
        private List<SelectListItem> GetInstrumentsForDropdown()
        {
            List<SelectListItem> instrumentList = new List<SelectListItem>();

            foreach (var instrument in config.Instruments)
            {
                SelectListItem item = new SelectListItem();
                item.Text = instrument.Value.Name;
                item.Value = instrument.Key;
                instrumentList.Add(item);
            }

            return instrumentList;
        }

        /// <summary>
        /// Gets a list of chord qualities to display in the dropdown on the UI.
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetChordQualitiesForDropdown()
        {
            List<SelectListItem> chordQualityList = new List<SelectListItem>();

            foreach (var quality in config.ChordQualities)
            {
                SelectListItem item = new SelectListItem();
                item.Text = quality.Value.Name;
                item.Value = quality.Key;
                chordQualityList.Add(item);
            }

            return chordQualityList;
        }

        /// <summary>
        /// Find the matching instrument object by the instrument name passed in.
        /// </summary>
        /// <param name="instrument">The instrument name.</param>
        /// <returns>The instrument object.</returns>
        private Instrument GetAnInstrument(string instrument)
        {
            Instrument myInstrument = (from i in config.Instruments
                                       where i.Value.Name.ToUpper() == instrument.ToUpper()
                                       select i.Value).FirstOrDefault();

            return myInstrument;
        }

        /// <summary>
        /// Find the matching chord quality object by the chord quality name passed in.
        /// </summary>
        /// <param name="chordQualities">Chord quality name.</param>
        /// <returns>The chord quality object.</returns>
        private ChordQuality GetChordQuality(string chordQualities)
        {
            ChordQuality myChordQuality = null;

            foreach (var quality in config.ChordQualities)
            {
                if (quality.Value.Name.ToUpper() == chordQualities.ToUpper())
                {
                    myChordQuality = new ChordQuality(quality.Value.Name, quality.Value.Abbreviation, quality.Value.Intervals);
                    break;
                }
            }

            return myChordQuality;
        }

        /// <summary>
        /// Find the tuning for the instrument by the instrument and tuning name passed in.
        /// </summary>
        /// <param name="instrument">The instrument object.</param>
        /// <param name="tuning">The tuning name.</param>
        /// <returns>The tuning object.</returns>
        private Tuning GetTheTuning(Instrument instrument, string tuning)
        {
            Tuning myTuning = (from t in instrument.Tunings
                               where t.LongName.ToUpper() == tuning.ToUpper()
                               select t).FirstOrDefault();

            return myTuning;
        }

        /// <summary>
        /// Builds the chord finder options with the parameters passed in.
        /// </summary>
        /// <param name="instrument">The instrument name.</param>
        /// <param name="tuning">The tuning for the instrument.</param>
        /// <param name="numFrets">The number of frets.</param>
        /// <param name="maxFrets">The maximum number of frets.</param>
        /// <param name="maxReach">The maximum reach of the fingers.</param>
        /// <param name="autoAddBarres">Auto add barres?</param>
        /// <param name="allowOpenStrings">Allow open strings?</param>
        /// <param name="allowMutedStrings">Allow muted strings?</param>
        /// <param name="mirrorResults">Mirror the results for left-handed chords?</param>
        /// <param name="allowRootlessChords">Allow rootless chords?</param>
        /// <returns>The chord finder object.</returns>
        private ChordFinderOptions BuildChordFinderOptions(string instrument, string tuning, string numFrets, string maxFrets, string maxReach, string autoAddBarres, string allowOpenStrings, string allowMutedStrings, string mirrorResults, string allowRootlessChords)
        {
            int myNumFrets = 5;
            int myMaxFrets = 12;
            int myMaxReach = 4;
            bool myAutoAddBarres = true;
            bool myAllowOpenStrings = true;
            bool myAllowMutedStrings = true;
            bool myAllowRootlessChords = false;
            bool myMirrorResults = false;

            Int32.TryParse(numFrets, out myNumFrets);
            Int32.TryParse(maxFrets, out myMaxFrets);
            Int32.TryParse(maxReach, out myMaxReach);
            bool.TryParse(autoAddBarres, out myAutoAddBarres);
            bool.TryParse(allowOpenStrings, out myAllowOpenStrings);
            bool.TryParse(allowMutedStrings, out myAllowMutedStrings);
            bool.TryParse(allowRootlessChords, out myAllowRootlessChords);
            bool.TryParse(mirrorResults, out myMirrorResults);

            ChordFinderOptions myOptions = new ChordFinderOptions(myNumFrets, myMaxFrets, myMaxReach, myAutoAddBarres, myAllowOpenStrings, myAllowMutedStrings, myAllowRootlessChords, myMirrorResults);
            return myOptions;
        }

        /// <summary>
        /// Create a chord options object and set it's default values.
        /// </summary>
        /// <returns>The chord options object.</returns>
        private ChordOptions BuildChordOptions()
        {
            ChordOptions chordOptions = new ChordOptions();
            chordOptions.BarreType = BarreType.Arc;
            chordOptions.OpenStringType = OpenStringType.Circle;
            chordOptions.Width = 150;
            chordOptions.Height = 226;
            return chordOptions;
        }

        /// <summary>
        /// Build the list of SVG images to display.
        /// </summary>
        /// <param name="chordResultSet">The chord result set.</param>
        /// <param name="chordOptions">The chord options.</param>
        /// <returns>The list of SVG images.</returns>
        private List<string> BuildSVGList(ChordResultSet chordResultSet, ChordOptions chordOptions)
        {
            // Define the collections.
            List<string> svgList = new List<string>();
            List<Chord> myChords = new List<Chord>();

            for (int i = 0; i < chordResultSet.Count; i++)
            {
                myChords.Add(chordResultSet.ChordAt(i));
            }

            foreach (Chord chord in myChords)
            {
                svgList.Add(chord.ToSvg(chordOptions));
            }

            return svgList;
        }

        #endregion Private Methods
    }
}