// Comprehensive Log Investigator for Mission Planner
// Created by Nadav Golan-Yanay
// Version 0.5.7.1
// Expected menu: LOG INVESTIGATOR

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using MissionPlanner;
using MissionPlanner.Controls;
using MissionPlanner.Log;
using MissionPlanner.Utilities;

namespace ComprehensiveLogInvestigatorV0571HebrewPolishPlugin
{
    internal static class UiPalette
    {
        // Dark charcoal surfaces with deliberately high text contrast. The theme is
        // dark enough to reduce glare but avoids pure black, which improves long-log readability.
        public static readonly Color Navy = Color.FromArgb(15, 23, 34);
        public static readonly Color NavyLight = Color.FromArgb(28, 41, 57);
        public static readonly Color Accent = Color.FromArgb(77, 163, 255);
        public static readonly Color AccentDark = Color.FromArgb(42, 116, 194);
        public static readonly Color AccentHover = Color.FromArgb(96, 177, 255);
        public static readonly Color AppBackground = Color.FromArgb(22, 27, 36);
        public static readonly Color Surface = Color.FromArgb(31, 38, 49);
        public static readonly Color SurfaceAlt = Color.FromArgb(36, 44, 56);
        public static readonly Color SurfaceRaised = Color.FromArgb(43, 52, 66);
        public static readonly Color PlotSurface = Color.FromArgb(25, 31, 41);
        public static readonly Color Border = Color.FromArgb(66, 77, 93);
        public static readonly Color Grid = Color.FromArgb(48, 58, 72);
        public static readonly Color GridMajor = Color.FromArgb(68, 81, 98);
        public static readonly Color Text = Color.FromArgb(239, 244, 250);
        public static readonly Color TextStrong = Color.White;
        public static readonly Color Muted = Color.FromArgb(174, 186, 201);
        public static readonly Color Disabled = Color.FromArgb(116, 128, 143);
        public static readonly Color Critical = Color.FromArgb(255, 119, 126);
        public static readonly Color CriticalSoft = Color.FromArgb(83, 40, 47);
        public static readonly Color Warning = Color.FromArgb(255, 193, 97);
        public static readonly Color WarningSoft = Color.FromArgb(79, 60, 31);
        public static readonly Color Advisory = Color.FromArgb(101, 195, 255);
        public static readonly Color AdvisorySoft = Color.FromArgb(34, 64, 86);
        public static readonly Color Success = Color.FromArgb(104, 222, 157);
        public static readonly Color SuccessSoft = Color.FromArgb(31, 72, 54);
        public static readonly Color Selection = Color.FromArgb(53, 83, 116);
        public static readonly Color TooltipBackground = Color.FromArgb(244, 35, 43, 55);
        public static readonly Color TooltipBorder = Color.FromArgb(111, 129, 151);
        public static readonly Color Replay = Color.FromArgb(255, 92, 214);

        public static void StyleButton(Button button, bool primary)
        {
            button.UseVisualStyleBackColor = false;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = primary ? 0 : 1;
            button.FlatAppearance.BorderColor = primary ? Accent : Border;
            button.FlatAppearance.MouseOverBackColor = primary ? AccentHover : SurfaceRaised;
            button.FlatAppearance.MouseDownBackColor = primary ? AccentDark : NavyLight;
            button.BackColor = primary ? AccentDark : SurfaceRaised;
            button.ForeColor = TextStrong;
            button.Font = new Font("Segoe UI", 9.0f, primary ? FontStyle.Bold : FontStyle.Regular);
            button.Height = 32;
            button.Padding = new Padding(10, 0, 10, 0);
            button.Cursor = Cursors.Hand;
        }

        public static void StyleCombo(ComboBox combo)
        {
            combo.FlatStyle = FlatStyle.Flat;
            combo.BackColor = SurfaceRaised;
            combo.ForeColor = Text;
            combo.Font = new Font("Segoe UI", 9.0f);
        }

        public static void StyleCheckBox(CheckBox checkBox)
        {
            checkBox.UseVisualStyleBackColor = false;
            checkBox.BackColor = Surface;
            checkBox.ForeColor = Text;
            checkBox.Font = new Font("Segoe UI", 8.8f);
        }
    }

    internal static class Localization
    {
        private static readonly Dictionary<string, string> UiEnglishToHebrew = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "DRONE LOG INVESTIGATOR", "חוקר לוגי טיסה" },
            { "Clear, evidence-based ArduPilot flight investigation inside Mission Planner", "ניתוח לוגי ArduPilot מבוסס ראיות בתוך Mission Planner" },
            { "Open logs", "פתח לוגים" },
            { "Analyze again", "נתח שוב" },
            { "Open raw log", "פתח לוג גולמי" },
            { "Export report", "ייצא דוח" },
            { "Selected log", "לוג נבחר" },
            { "Ready — open one or more ArduPilot logs to begin the investigation.", "מוכן — פתח לוג ArduPilot אחד או יותר כדי להתחיל בחקירה." },
            { "Overview", "סקירה" },
            { "Incidents", "אירועים חריגים" },
            { "Technical summary", "סיכום טכני" },
            { "Findings", "ממצאים" },
            { "Timeline", "ציר זמן" },
            { "Pilot & GCS", "טייס / GCS" },
            { "Graphs", "גרפים" },
            { "Interactive Map", "מפה" },
            { "Values", "ערכים" },
            { "Log data", "נתוני לוג" },
            { "Info", "מידע" },
            { "Graph:", "גרף:" },
            { "Reset view", "אפס תצוגה" },
            { "Show all", "הצג הכל" },
            { "Values view:", "תצוגת ערכים:" },
            { "No log selected.", "לא נבחר לוג." },
            { "Snapshot follows map / graph / incident time. Normalized signals use canonical engineering units without magnitude guessing.", "התצוגה עוקבת אחר זמן המפה / הגרף / האירוע. אותות מנורמלים משתמשים ביחידות הנדסיות ללא ניחוש לפי גודל הערך." },
            { "PILOT INPUTS AND GCS / MAVLINK COMMANDS", "קלטי טייס ופקודות GCS / MAVLINK" },
            { "GROUND-STATION / MAVLINK COMMAND HISTORY", "היסטוריית פקודות תחנת קרקע / MAVLINK" },
            { "RCIN shows the control inputs seen by the autopilot. RCI2 identifies MAVLink RC override channels. MAVC lists executed MAVLink commands when the firmware logged them.", "RCIN מציג את קלטי השליטה שנראו על ידי הטייס האוטומטי. RCI2 מזהה ערוצי MAVLink RC override. MAVC מציג פקודות MAVLink שבוצעו כאשר הקושחה תיעדה אותן." },
            { "Mouse wheel: zoom   •   drag: pan   •   click: seek   •   legend: show/hide", "גלגלת עכבר: זום   •   גרירה: הזזה   •   לחיצה: מעבר בזמן   •   מקרא: הצג/הסתר" },
            { "Play", "נגן" },
            { "Pause", "השהה" },
            { "Zoom +", "זום +" },
            { "Zoom -", "זום -" },
            { "Load a log with GPS data to use interactive replay.", "טען לוג עם נתוני GPS כדי להשתמש בניגון האינטראקטיבי." },
            { "No GPS sample near this time for the selected receiver.", "אין דגימת GPS סמוכה לזמן זה עבור המקלט שנבחר." },
            { "Selected range: full log", "טווח נבחר: כל הלוג" },
            { "Range start: ", "תחילת טווח: " },
            { "Range end: ", "סוף טווח: " },
            { "Range: ", "טווח: " },
            { "Cursor ", "סמן " },
            { "Created by Nadav Golan-Yanay  •  © 2026 Nadav Golan-Yanay. All rights reserved.", "נוצר על ידי Nadav Golan-Yanay  •  © 2026 Nadav Golan-Yanay. כל הזכויות שמורות." },
            { "Analysis stopped because of an error.", "הניתוח הופסק עקב שגיאה." },
            { "Severity", "חומרה" },
            { "Event confidence", "ביטחון בזיהוי" },
            { "Cause confidence", "ביטחון בסיבה" },
            { "File", "קובץ" },
            { "Start", "התחלה" },
            { "Phase", "שלב" },
            { "Category", "קטגוריה" },
            { "Principal incident", "אירוע מרכזי" },
            { "Commanded?", "התאמה לפקודה" },
            { "Correlated evidence", "ראיות מתואמות" },
            { "Time", "זמן" },
            { "Source", "מקור" },
            { "Target", "יעד" },
            { "Command", "פקודה" },
            { "Result", "תוצאה" },
            { "Method", "שיטה" },
            { "Parameters", "פרמטרים" },
            { "Position / XYZ", "מיקום / XYZ" },
            { "Line", "שורה" },
            { "Signal / value", "אות / ערך" },
            { "Value", "ערך" },
            { "Unit", "יחידה" },
            { "Sample time", "זמן דגימה" },
            { "Age", "גיל דגימה" },
            { "Interpretation / normalization", "פירוש / נרמול" },
            { "Confidence", "רמת ביטחון" },
            { "Subsystem", "תת-מערכת" },
            { "Finding", "ממצא" },
            { "Evidence", "ראיה" },
            { "Recommendation", "המלצה" },
            { "Type", "סוג" },
            { "Text", "טקסט" },
            { "Message", "הודעה" },
            { "Count", "כמות" },
            { "Fields", "שדות" },
            { "Reverse", "אחורה" },
            { "Fit track", "התאם מסלול" },
            { "Follow vehicle", "עקוב אחר הכלי" },
            { "Events", "אירועים" },
            { "Future path", "מסלול עתידי" },
            { "Speed:", "מהירות:" },
            { "Waypoints", "נקודות משימה" },
            { "Mode changes", "שינויי מצב" },
            { "References", "נקודות ייחוס" },
            { "Set start", "קבע התחלה" },
            { "Set end", "קבע סוף" },
            { "Clear range", "נקה טווח" },
            { "Range only", "טווח בלבד" },
            { "GPS:", "GPS:" },
            { "All GPS receivers", "כל מקלטי ה-GPS" },
            { "No GPS track", "אין מסלול GPS" },
            { "No valid GPS coordinates", "אין קואורדינטות GPS תקינות" },
            { "No valid GPS track available for the selected receiver", "אין מסלול GPS תקין עבור המקלט שנבחר" },
            { "LIVE PILOT INPUT REPLAY", "קלטי טייס לאורך ציר הזמן" },
            { "Open a log to inspect pilot commands.", "פתח לוג כדי לבדוק פקודות טייס." },
            { "No RCIN records were found. Enable RC input logging to see stick movement.", "לא נמצאו רשומות RCIN. הפעל רישום קלט RC כדי לראות תנועת סטיקים." },
            { "No RC input sample is available at this time.", "אין דגימת קלט RC זמינה בזמן זה." },
            { "LEFT STICK", "סטיק שמאלי" },
            { "RIGHT STICK", "סטיק ימני" },
            { "Yaw", "סבסוב" },
            { "Throttle", "מצערת" },
            { "Roll", "גלגול" },
            { "Pitch", "עלרוד" },
            { "Flight phase", "שלב טיסה" },
            { "RC override mask", "מסכת RC override" }
        };

        public static string Ui(string text, bool hebrew)
        {
            if (string.IsNullOrEmpty(text)) return text;
            if (hebrew)
            {
                string value;
                return UiEnglishToHebrew.TryGetValue(text, out value) ? value : text;
            }
            foreach (KeyValuePair<string, string> pair in UiEnglishToHebrew)
                if (string.Equals(pair.Value, text, StringComparison.Ordinal)) return pair.Key;
            return text;
        }

        public static string Severity(string value)
        {
            if (value == "Critical") return "קריטי";
            if (value == "Warning") return "אזהרה";
            if (value == "Advisory") return "הערה";
            return value ?? string.Empty;
        }

        public static string Confidence(string value)
        {
            if (value == "High") return "גבוהה";
            if (value == "Medium") return "בינונית";
            if (value == "Low") return "נמוכה";
            return value ?? string.Empty;
        }

        public static string Phase(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            switch (value)
            {
                case "Disarmed": return "לא חמוש";
                case "Armed on ground": return "חמוש על הקרקע";
                case "Takeoff": return "המראה";
                case "Airborne": return "באוויר";
                case "Landing": return "נחיתה";
                case "Landed": return "נחת";
                case "Abnormal or uncertain ending": return "סיום חריג או לא ודאי";
                case "Whole log": return "כל הלוג";
                default: return value;
            }
        }

        public static string Command(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            switch (value)
            {
                case "Uncommanded": return "ללא פקודה תואמת";
                case "Pilot-commanded": return "תואם קלט טייס";
                case "Autopilot/GCS-commanded": return "תואם פקודת אוטופיילוט / GCS";
                case "Failsafe-commanded": return "תואם פעולת Failsafe";
                case "Uncertain": return "לא ניתן לקבוע";
                default: return value;
            }
        }

        public static string Analysis(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            var exact = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Outcome uncertain because the log file is physically incomplete", "לא ניתן לקבוע בוודאות את תוצאת הטיסה משום שקובץ הלוג קטוע" },
                { "Outcome uncertain because log integrity or parsing is incomplete", "לא ניתן לקבוע בוודאות את תוצאת הטיסה משום שהלוג או תהליך הפענוח אינם שלמים" },
                { "Abnormal log ending without enough evidence to assign a root cause", "הלוג הסתיים באופן חריג, ללא די ראיות לקביעת הסיבה הראשונית" },
                { "Uncertain ending", "סיום לא ודאי" },
                { "Probable incident with an abnormal or uncertain ending", "אירוע סביר עם סיום חריג או לא ודאי" },
                { "Probable incident identified by correlated evidence", "זוהה אירוע בעל תמיכה ראייתית ממספר מקורות מתואמים" },
                { "Normal or apparently normal completed flight; no principal incident exceeded the correlation threshold", "הטיסה הסתיימה באופן תקין או לכאורה תקין, ולא זוהה אירוע מרכזי שעבר את סף הראיות" },
                { "Flight outcome is uncertain from the available state evidence", "תוצאת הטיסה אינה ודאית על סמך ראיות מצב הטיסה הזמינות" },
                { "Control response / attitude divergence", "חריגה בתגובת הבקרה / סטייה במצב הזוויתי" },
                { "Probable uncommanded attitude divergence", "סטייה חריגה במצב הזוויתי ללא פקודה תואמת" },
                { "Commanded aggressive maneuver with elevated tracking error", "תמרון אגרסיבי תואם-פקודה עם שגיאת עקיבה גבוהה" },
                { "Correlated GPS/EKF navigation degradation", "הידרדרות ניווט GPS/EKF מתואמת" },
                { "GPS/navigation degradation", "הידרדרות GPS/ניווט" },
                { "Estimator degradation", "הידרדרות באומד המצב" },
                { "Battery/power event", "אירוע חריג במערכת הסוללה / ההספק" },
                { "RC or GCS failsafe / command-path event", "אירוע Failsafe או נתיב פקודה של RC/GCS" },
                { "Rough landing / ground-contact event", "נחיתה קשה / מגע חריג בקרקע" },
                { "Collision, rollover, or severe terminal-motion evidence", "ראיות להתנגשות, התהפכות או תנועה חריגה וחמורה בסיום האירוע" },
                { "No principal incident was identified from the available correlated evidence.", "לא זוהה אירוע מרכזי מתוך הראיות המתואמות הזמינות." },
                { "No composite incident reached the evidence threshold. Individual findings remain available in the Findings tab.", "אף אירוע משולב לא הגיע לסף הראיות. ממצאים נפרדים זמינים בלשונית הממצאים." },
                { "No high-value event records were available for a concise timeline.", "לא היו רשומות אירועים בעלות ערך גבוה לציר זמן תמציתי." },
                { "The available data is not strong enough to positively reduce the likelihood of a specific subsystem cause.", "הנתונים הזמינים אינם חזקים מספיק כדי להפחית בביטחון את הסבירות לגורם מתת-מערכת מסוימת." },
                { "These statements reduce likelihood only; they are not proof that a subsystem was fault-free.", "הצהרות אלו רק מפחיתות הסתברות; הן אינן הוכחה שתת-המערכת הייתה חפה מתקלה." },
                { "No root cause can be ranked reliably from this log alone.", "לא ניתן לדרג באופן אמין את הסיבה הראשונית על סמך הלוג הזה בלבד." },
                { "No major data limitation was identified for the questions assessed by this report.", "לא זוהתה מגבלת נתונים משמעותית עבור השאלות שנבדקו בדוח זה." },
                { "No major incident-specific data limitation was identified.", "לא זוהתה מגבלת נתונים משמעותית הספציפית לאירוע." },
                { "Compare this flight with a known-good flight using the same vehicle, firmware and configuration, and verify any remaining questions in raw Log Browse.", "השווה טיסה זו לטיסה תקינה מוכרת עם אותו כלי, קושחה ותצורה, ואמת שאלות שנותרו ב-Log Browse הגולמי." },
                { "not reliably available", "לא זמין באופן אמין" },
                { "not available", "לא זמין" },
                { "Unknown", "לא ידוע" },
                { "Uncertain", "לא ודאי" },
                { "Incomplete", "לא שלם" },
                { "Abnormal", "חריג" },
                { "Normal", "תקין" },
                { "Vibration", "רעידות" },
                { "Attitude control", "בקרת מצב זוויתי" },
                { "Log integrity", "שלמות הלוג" },
                { "Data availability", "זמינות נתונים" },
                { "Battery", "סוללה" },
                { "Measured motor performance", "ביצועי מנוע נמדדים" },
                { "High in-flight vibration or accelerometer clipping was recorded", "נרשמו רעידות חריגות בטיסה ו/או clipping במד התאוצה" },
                { "Large desired-versus-actual roll error", "פער גדול בין זווית הגלגול הרצויה לזווית שנמדדה" },
                { "Large desired-versus-actual pitch error", "פער גדול בין זווית העלרוד הרצויה לזווית שנמדדה" },
                { "Log ending classified as Incomplete", "סיום הלוג סווג כלא שלם" },
                { "The binary file ends before the declared length of the final DataFlash record.", "הקובץ הבינרי מסתיים לפני האורך המוגדר של רשומת ה-DataFlash האחרונה." },
                { "The message format exists in the log, but no usable flight records were found. The source may be inactive, unused, or excluded by the runtime logging profile.", "פורמט ההודעה קיים בלוג, אך לא נמצאו רשומות טיסה שימושיות. ייתכן שהמקור לא היה פעיל, לא היה בשימוש או לא נכלל בפרופיל הרישום בזמן הטיסה." },
                { "Check subsystem activity/selection first; only then review the logging profile.", "בדוק תחילה אם תת-המערכת הייתה פעילה ונבחרה לשימוש, ורק לאחר מכן בדוק את פרופיל הרישום." },
                { "Inspect propellers, motors, bearings, frame stiffness and flight-controller isolation. Correlate onset with control error and motor output before treating vibration as the initiating cause.", "בדוק מדחפים, מנועים, מיסבים, קשיחות שלדה ובידוד בקר הטיסה. השווה את תחילת הרעידות לשגיאת הבקרה ולפקודות המנוע לפני קביעה שהרעידות הן הסיבה הראשונית." },
                { "Check for motor saturation, wind/load limits, mechanical asymmetry, tuning issues and sensor problems at the same timestamp.", "בדוק רוויה בפקודות המנוע, מגבלות רוח או עומס, אסימטריה מכנית, כוונון וחיישנים סביב אותו זמן." },
                { "Verify the raw end of the log and the vehicle's final state. Do not assume power loss unless independent power evidence supports it; file/download truncation and interrupted logging remain alternatives.", "בדוק את הרשומות הגולמיות בסוף הלוג ואת מצבו הסופי של הכלי. אין להסיק אובדן מתח ללא ראיות עצמאיות ממערכת ההספק; קובץ קטוע או הפסקת רישום הן עדיין אפשרויות." },
                { "RATE data was unavailable or insufficient for detailed control-effectiveness analysis; attitude tracking and motor-command evidence were used instead.", "נתוני RATE לא היו זמינים או לא הספיקו לניתוח מפורט של יעילות הבקרה; במקום זאת נעשה שימוש בעקיבת המצב הזוויתי ובפקודות המנוע." }
            };
            string found;
            if (exact.TryGetValue(text, out found)) return found;
            string probe = text.TrimEnd();
            bool trailingPeriod = probe.EndsWith(".", StringComparison.Ordinal);
            if (trailingPeriod)
            {
                probe = probe.Substring(0, probe.Length - 1);
                if (exact.TryGetValue(probe, out found)) return found + ".";
            }

            // Phrase-level replacements keep technical names, units, parameters and MAVLink identifiers intact.
            string t = text;
            string[,] pairs = new string[,]
            {
                { "Principal incident: ", "אירוע מרכזי: " },
                { "Confidence the event occurred: ", "ביטחון בזיהוי האירוע: " },
                { "Confidence in the proposed root cause: ", "ביטחון בסיבה הראשונית המוצעת: " },
                { "Log/parser limitation: ", "מגבלת לוג/מפענח: " },
                { "Armed duration: ", "משך חמוש: " },
                { "Airborne duration: ", "משך באוויר: " },
                { "Modes used: ", "מצבי טיסה: " },
                { "Altitude range relative to the logged reference: ", "טווח גובה ביחס לייחוס המתועד: " },
                { "Maximum height above the armed-ground reference: ", "גובה מרבי מעל ייחוס הקרקע בזמן החימוש: " },
                { "Maximum ground speed (selected primary GPS): ", "מהירות קרקע מרבית (GPS ראשי נבחר): " },
                { "Maximum distance from the first valid armed primary-GPS point: ", "מרחק מרבי מנקודת ה-GPS הראשית התקינה הראשונה בזמן חימוש: " },
                { "Final resolved state: ", "מצב סופי שנקבע: " },
                { "Log ending/integrity: ", "סיום/שלמות הלוג: " },
                { "Firmware interpretation: ", "זיהוי קושחה: " },
                { "What happened: ", "מה קרה: " },
                { "Start: ", "התחלה: " },
                { "; peak: ", "; שיא: " },
                { "; phase: ", "; שלב: " },
                { "Peak severity: ", "חומרת שיא: " },
                { "composite score ", "ציון משולב " },
                { "Likely consequences: ", "השלכות אפשריות: " },
                { "Command classification: ", "התאמה לפקודות ולקלטים: " },
                { "Classification: ", "סיווג: " },
                { "The tool does not assign blame unless command and response evidence agree.", "הכלי אינו מייחס אשמה אלא אם ראיות הפקודה והתגובה תואמות." },
                { "executed ", "בוצעה " },
                { " from ", " מ-" },
                { "; result ", "; תוצאה " },
                { "pilot input roll ", "קלט טייס גלגול " },
                { ", pitch ", ", עלרוד " },
                { ", throttle ", ", מצערת " },
                { ", yaw ", ", סבסוב " },
                { "; MAVLink RC override active.", "; MAVLink RC override פעיל." },
                { "Limiting evidence: ", "מגבלת ראיות: " },
                { "root-cause confidence: ", "ביטחון בסיבה הראשונית: " },
                { "Monitoring disabled", "ניטור מושבת" },
                { "Sensor not installed", "החיישן אינו מותקן" },
                { "Configured but unused", "מוגדר אך אינו בשימוש" },
                { "No usable data despite declared format", "אין נתונים שימושיים למרות פורמט מוגדר" },
                { "No ESC/RPM telemetry was logged", "לא תועדה טלמטריית ESC/RPM" },
                { "physical motor/propeller response cannot be verified", "לא ניתן לאמת את תגובת המנוע/מדחף בפועל" },
                { "RCOU shows commanded output, not measured thrust or RPM", "RCOU מציג פלט פקוד ולא דחף או RPM נמדדים" },
                { "Battery voltage/current behavior cannot be used to confirm or exclude a battery-related initiating event", "לא ניתן להשתמש בהתנהגות מתח/זרם הסוללה כדי לאשר או לשלול אירוע התחלתי הקשור לסוללה" },
                { "battery monitoring data is not available for this flight", "נתוני ניטור הסוללה אינם זמינים לטיסה זו" },
                { "The log ending is classified as Incomplete", "סיום הלוג מסווג כלא שלם" },
                { "the final sequence may be incomplete", "ייתכן שהרצף הסופי אינו שלם" },
                { "which limits reconstruction of the event ending and recovery state", "דבר המגביל את שחזור סיום האירוע ומצב ההתאוששות" },
                { "Download/file truncation or interrupted logging is possible; this does not prove onboard power loss.", "ייתכן שקובץ ההורדה נקטע או שהרישום הופסק; אין בכך הוכחה לאובדן מתח בכלי." },
                { "Primary GPS produced sufficient active, internally consistent data", "ה-GPS הראשי סיפק די נתונים פעילים ועקביים פנימית" },
                { "no correlated GPS/navigation failure was identified", "לא זוהה כשל GPS/ניווט מתואם" },
                { "this evidence reduces the likelihood of primary-GPS loss as the initiating cause", "ראיה זו מפחיתה את הסבירות שאובדן ה-GPS הראשי היה הגורם המתחיל" },
                { "Estimator data was available without a correlated estimator-divergence incident", "נתוני האומד היו זמינים ללא אירוע סטייה מתואם של האומד" },
                { "this evidence reduces the likelihood of an isolated EKF failure", "ראיה זו מפחיתה את הסבירות לכשל EKF מבודד" },
                { "An active compass produced adequate data without a correlated compass fault", "מצפן פעיל סיפק נתונים מספקים ללא תקלה מתואמת במצפן" },
                { "Barometer data coverage was adequate without a correlated barometer fault", "כיסוי נתוני הברומטר היה מספק ללא תקלה מתואמת בברומטר" },
                { "The parser completed without a reported error", "המפענח סיים ללא שגיאה מדווחת" },
                { "this supports basic log readability", "הדבר תומך בקריאות בסיסית של הלוג" },
                { "although it does not by itself prove the file ended normally", "אך אינו מוכיח כשלעצמו שהקובץ הסתיים באופן תקין" },
                { "Physical motor/propeller response cannot be verified because usable ESC/RPM telemetry is unavailable", "לא ניתן לאמת את תגובת המנוע/מדחף בפועל משום שטלמטריית ESC/RPM שימושית אינה זמינה" },
                { "Commanded motor/servo output can be assessed from RCOU when present", "ניתן להעריך פלט מנוע/סרוו פקוד מ-RCOU כאשר הוא קיים" },
                { "but physical motor response cannot be verified without usable ESC/RPM telemetry", "אך לא ניתן לאמת תגובת מנוע בפועל ללא טלמטריית ESC/RPM שימושית" },
                { "Desired-versus-actual attitude error grew to ", "שגיאת המצב הזוויתי בין הרצוי לנמדד גדלה ל-" },
                { "Peak attitude tracking error ", "שגיאת עקיבת מצב זוויתי בשיא " },
                { "Actual attitude became large while desired attitude and available pilot input remained comparatively small.", "המצב הזוויתי בפועל גדל בעוד שהמצב הרצוי וקלט הטייס הזמין נותרו קטנים יחסית." },
                { "Extreme attitude ", "מצב זוויתי קיצוני " },
                { "Absolute roll/pitch reached approximately ", "ערך מוחלט של גלגול/עלרוד הגיע לכ-" },
                { "Altitude evidence placed the event near the ground", "ראיות הגובה מציבות את האירוע סמוך לקרקע" },
                { "IMU angular rate exceeded approximately ", "קצב זוויתי של IMU עבר בקירוב " },
                { "Accelerometer clipping increased by ", "מונה clipping של מד התאוצה גדל ב-" },
                { "The event was close to the log ending or an abnormal/uncertain resolved ending.", "האירוע התרחש סמוך לסיום הלוג או לסיום חריג/לא ודאי שנקבע." },
                { "Multiple independent signals are consistent with severe physical motion/contact", "מספר אותות בלתי תלויים תואמים לתנועה פיזית/מגע חמורים" },
                { "this is stronger evidence than attitude magnitude alone", "זוהי ראיה חזקה יותר מגודל הזווית לבדו" },
                { "Ground/obstacle contact or rollover after descent/loss of control.", "מגע בקרקע או במכשול, או התהפכות לאחר הנמכה או אובדן שליטה." },
                { "A preceding control, propulsion, navigation/estimator, or commanded maneuver event", "אירוע מוקדם יותר במערכת הבקרה, ההנעה, הניווט/האומד או תמרון שנדרש מהכלי" },
                { "inspect earlier onset before treating impact evidence as root cause", "יש לבדוק את תחילת האירוע המוקדמת לפני שמייחסים לראיות הפגיעה את הסיבה הראשונית" },
                { "Possible ground/obstacle contact, rollover or termination of controlled flight.", "ייתכן מגע בקרקע או במכשול, התהפכות או אובדן טיסה נשלטת." },
                { "Prioritize the 5–10 s before this event for root-cause analysis", "בניתוח הסיבה הראשונית יש להתמקד קודם כול ב-5–10 השניות שלפני האירוע" },
                { "impact vibration/clipping that begins afterward is consequence evidence", "רעידות/clipping שמתחילים לאחר מכן הם ראיות לתוצאה ולא בהכרח לסיבה" },
                { "Detailed findings: ", "ממצאים מפורטים: " },
                { "Principal incidents: ", "אירועים מרכזיים: " },
                { "Pilot-input samples: ", "דגימות קלט טייס: " },
                { "Executed MAVLink commands: ", "פקודות MAVLink שבוצעו: " },
                { "Detailed telemetry, secondary findings and raw evidence remain available in the Incidents, Findings, Timeline, Pilot & GCS, Graphs, Interactive Map, Values and Log data tabs.", "טלמטריה מפורטת, ממצאים נוספים וראיות גולמיות זמינים בלשוניות אירועים, ממצאים, ציר זמן, טייס/GCS, גרפים, מפה, ערכים ונתוני לוג." },
                { "standard ArduPilot", "ArduPilot תקני" },
                { "interpretation confidence ", "רמת ביטחון בזיהוי " },
                { "custom/modified build", "גרסה מותאמת או שונה" },
                { "unknown build", "גרסה לא מזוהה" },
                { "High vibration was already present before control-error onset", "רעידות גבוהות כבר הופיעו לפני תחילת שגיאת הבקרה" },
                { "pre-onset peak ", "שיא לפני תחילת האירוע " },
                { "so it remains a plausible contributing factor.", "ולכן הן עדיין עשויות להיות גורם תורם." },
                { "Stable-flight 95th-percentile vibration ", "אחוזון 95 של הרעידות בקטע טיסה יציב: " },
                { "; peak ", "; שיא " },
                { "; clipping increase ", "; עלייה במונה clipping: " },
                { "95th-percentile error ", "אחוזון 95 של השגיאה: " },
                { "Compare the same signals with a known-good flight using the same vehicle, firmware and configuration before declaring a root cause.", "השווה את אותם האותות לטיסה תקינה מוכרת עם אותו כלי, אותה קושחה ואותה תצורה לפני קביעת הסיבה הראשונית." }
            };
            for (int i = 0; i < pairs.GetLength(0); i++) t = t.Replace(pairs[i,0], pairs[i,1]);
            t = t.Replace("High", "גבוהה").Replace("Medium", "בינונית").Replace("Low", "נמוכה");
            t = t.Replace("Critical", "קריטי").Replace("Warning", "אזהרה").Replace("Advisory", "הערה");
            t = t.Replace("Uncommanded", "ללא פקודה תואמת").Replace("Pilot-commanded", "תואם קלט טייס").Replace("Failsafe-commanded", "תואם פעולת Failsafe");
            return t;
        }

        public static string Report(string english)
        {
            if (string.IsNullOrEmpty(english)) return english;
            string[] lines = english.Replace("\r\n", "\n").Split('\n');
            var output = new List<string>();
            foreach (string raw in lines)
            {
                string line = raw;
                switch (line)
                {
                    case "PLAIN-LANGUAGE FLIGHT STORY": line = "סיכום חקירת הטיסה"; break;
                    case "Created by Nadav Golan-Yanay": line = "נוצר על ידי Nadav Golan-Yanay"; break;
                    case "© 2026 Nadav Golan-Yanay. All rights reserved.": line = "© 2026 Nadav Golan-Yanay. כל הזכויות שמורות."; break;
                    case "1. OVERALL VERDICT": line = "1. סיכום החקירה"; break;
                    case "2. FLIGHT OUTCOME": line = "2. סיכום הטיסה"; break;
                    case "3. PRINCIPAL INCIDENT": line = "3. האירוע המרכזי"; break;
                    case "4. EVIDENCE TIMELINE": line = "4. ציר זמן ראייתי"; break;
                    case "5. WHAT WAS COMMANDED": line = "5. פקודות וקלטי שליטה"; break;
                    case "6. VEHICLE RESPONSE": line = "6. תגובת הכלי"; break;
                    case "7. SYSTEMS THAT APPEARED HEALTHY / REDUCED-LIKELIHOOD CAUSES": line = "7. מערכות ללא אינדיקציה לכשל / סיבות שפחות סבירות"; break;
                    case "8. POSSIBLE CAUSES RANKED BY EVIDENCE": line = "8. סיבות אפשריות לפי חוזק הראיות"; break;
                    case "9. MISSING OR LIMITING DATA": line = "9. נתונים חסרים ומגבלות הניתוח"; break;
                    case "10. RECOMMENDED NEXT DIAGNOSTIC STEPS": line = "10. בדיקות המשך מומלצות"; break;
                    case "TECHNICAL CONTEXT": line = "נתונים טכניים"; break;
                    case "Known from the log:": line = "עובדות המתועדות בלוג:"; break;
                    case "Strongly inferred:": line = "מסקנות הנתמכות היטב בראיות:"; break;
                    case "Possible causes:": line = "סיבות אפשריות:"; break;
                    case "Cannot be determined from available data:": line = "מה לא ניתן לקבוע מהלוג:"; break;
                    default:
                        if (line.StartsWith("Generated: ")) line = "נוצר בתאריך: " + line.Substring("Generated: ".Length);
                        else if (line.StartsWith("LIMITATION: ")) line = "מגבלה: " + Analysis(line.Substring(12));
                        else if (line.StartsWith("- ")) line = "- " + Analysis(line.Substring(2));
                        else
                        {
                            Match m = Regex.Match(line, @"^(\d+)\.\s+(.*)$");
                            if (m.Success) line = m.Groups[1].Value + ". " + Analysis(m.Groups[2].Value);
                            else line = Analysis(line);
                        }
                        break;
                }
                output.Add(line);
            }
            return string.Join(Environment.NewLine, output.ToArray());
        }

        public static string PlainText(string english)
        {
            if (string.IsNullOrEmpty(english)) return english;
            string t = english;
            string[,] pairs = new string[,]
            {
                { "COMPREHENSIVE LOG INVESTIGATION", "חקירת לוג מקיפה" },
                { "Created by Nadav Golan-Yanay", "נוצר על ידי Nadav Golan-Yanay" },
                { "Generated: ", "נוצר בתאריך: " },
                { "This report is a diagnostic aid, not an airworthiness approval or proof of root cause.", "דוח זה הוא כלי עזר אבחוני ואינו אישור כשירות או הוכחה לסיבה הראשונית." },
                { "Path: ", "נתיב: " },
                { "Size: ", "גודל: " },
                { "Parsed lines: ", "שורות שפוענחו: " },
                { "Duration: ", "משך: " },
                { "Message types: ", "סוגי הודעות: " },
                { "RC input samples: ", "דגימות קלט RC: " },
                { "Executed MAVLink commands: ", "פקודות MAVLink שבוצעו: " },
                { "Findings: ", "ממצאים: " },
                { "Parser error: ", "שגיאת מפענח: " },
                { "Interpretation foundation:", "בסיס הפירוש:" },
                { "Firmware: ", "קושחה: " },
                { "Resolved final state: ", "מצב סופי שנקבע: " },
                { "Log ending/integrity: ", "סיום/שלמות הלוג: " },
                { "Sensor/configuration assessment:", "הערכת חיישנים/תצורה:" },
                { "Data availability:", "זמינות נתונים:" },
                { "Normalized signal groups: ", "קבוצות אותות מנורמלים: " },
                { "Correlated principal incidents:", "אירועים מרכזיים מתואמים:" },
                { "None identified from the available correlated evidence.", "לא זוהה אירוע מתוך הראיות המתואמות הזמינות." },
                { "Highest-priority findings:", "ממצאים בעדיפות גבוהה:" },
                { "Mode/events/messages:", "מצבים/אירועים/הודעות:" }
            };
            for (int i=0;i<pairs.GetLength(0);i++) t=t.Replace(pairs[i,0],pairs[i,1]);
            return Report(t);
        }
    }

    public sealed class Plugin : MissionPlanner.Plugin.Plugin
    {
        private const string ScreenName = "LogInvestigatorEmbedded";
        private ToolStripMenuItem _menuItem;
        private InvestigatorView _view;

        public override string Name { get { return "Comprehensive Log Investigator v0.5.7.1 HEBREW POLISH"; } }
        public override string Version { get { return "0.5.7.1"; } }
        public override string Author { get { return "Nadav Golan-Yanay"; } }

        public override bool Init()
        {
            loopratehz = 0.1f;
            return true;
        }

        public override bool Loaded()
        {
            Action addMenu = delegate
            {
                if (_menuItem != null)
                    return;

                _menuItem = new ToolStripMenuItem("LOG INVESTIGATOR");
                _menuItem.ToolTipText = "Open Log Investigator v0.5.7.1 HEBREW POLISH inside the Mission Planner workspace";
                _menuItem.Click += MenuItem_Click;

                if (MainV2.instance != null && MainV2.instance.MainMenuStrip != null)
                {
                    MainV2.instance.MainMenuStrip.Items.Add(_menuItem);
                    _menuItem.ForeColor = Color.FromArgb(190, 230, 60);
                    _menuItem.Font = new Font(SystemFonts.MenuFont, FontStyle.Bold);
                    _menuItem.Padding = new Padding(10, 0, 10, 0);
                }
            };

            if (MainV2.instance != null && MainV2.instance.InvokeRequired)
                MainV2.instance.BeginInvoke(addMenu);
            else
                addMenu();

            return true;
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!EnsureScreenRegistered())
                    throw new InvalidOperationException("Mission Planner's main screen switcher is not available yet.");

                MainV2.View.ShowScreen(ScreenName);
            }
            catch (Exception ex)
            {
                string diagnosticPath = WriteStartupDiagnostic(ex);
                string message = "The embedded Log Investigator screen could not be opened.\r\n\r\n" +
                                 ex.GetType().Name + ": " + ex.Message;
                if (!string.IsNullOrEmpty(diagnosticPath))
                    message += "\r\n\r\nA diagnostic file was saved to:\r\n" + diagnosticPath;

                MessageBox.Show(MainV2.instance, message,
                    "Log Investigator startup error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool EnsureScreenRegistered()
        {
            if (MainV2.View == null)
                return false;

            MainSwitcher.Screen existing = MainV2.View.screens
                .FirstOrDefault(screen => string.Equals(screen.Name, ScreenName, StringComparison.Ordinal));

            if (existing != null)
            {
                if (existing.Control != null && !existing.Control.IsDisposed)
                {
                    _view = existing.Control as InvestigatorView;
                    return _view != null;
                }

                MainV2.View.screens.Remove(existing);
            }

            _view = new InvestigatorView();
            MainV2.View.AddScreen(new MainSwitcher.Screen(ScreenName, _view, true));
            return true;
        }

        private static string WriteStartupDiagnostic(Exception ex)
        {
            try
            {
                string folder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "MissionPlanner", "LogInvestigator");
                Directory.CreateDirectory(folder);
                string path = Path.Combine(folder,
                    "startup_error_" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture) + ".txt");
                File.WriteAllText(path,
                    "Comprehensive Log Investigator v0.5.7.1 HEBREW POLISH\r\n" +
                    "Created by Nadav Golan-Yanay\r\n" +
                    "Generated: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + "\r\n\r\n" +
                    ex.ToString(), Encoding.UTF8);
                return path;
            }
            catch
            {
                return string.Empty;
            }
        }

        public override bool Loop()
        {
            return true;
        }

        public override bool Exit()
        {
            Action cleanup = delegate
            {
                try
                {
                    if (_menuItem != null)
                    {
                        _menuItem.Click -= MenuItem_Click;
                        if (_menuItem.Owner != null)
                            _menuItem.Owner.Items.Remove(_menuItem);
                        _menuItem.Dispose();
                        _menuItem = null;
                    }

                    if (MainV2.View != null)
                    {
                        MainSwitcher.Screen screen = MainV2.View.screens
                            .FirstOrDefault(item => string.Equals(item.Name, ScreenName, StringComparison.Ordinal));

                        if (screen != null)
                        {
                            if (ReferenceEquals(MainV2.View.current, screen))
                            {
                                MainSwitcher.Screen fallback = MainV2.View.screens.FirstOrDefault(item =>
                                    !ReferenceEquals(item, screen) &&
                                    ((item.Control != null && MainV2.instance != null &&
                                      ReferenceEquals(item.Control, MainV2.instance.FlightData)) ||
                                     (item.Type != null && item.Type.Name == "FlightData")));

                                MainV2.View.ShowScreen(fallback == null ? string.Empty : fallback.Name);
                            }

                            MainV2.View.screens.Remove(screen);
                            if (screen.Control != null && !screen.Control.IsDisposed)
                            {
                                screen.Control.Close();
                                screen.Control.Dispose();
                            }
                        }
                    }

                    _view = null;
                }
                catch
                {
                    // Plugin unload should not interrupt Mission Planner shutdown.
                }
            };

            if (MainV2.instance != null && MainV2.instance.InvokeRequired)
                MainV2.instance.BeginInvoke(cleanup);
            else
                cleanup();

            return true;
        }
    }

    [PreventTheming]
    internal sealed class InvestigatorView : MyUserControl, IActivate, IDeactivate
    {
        private readonly Button _openButton;
        private readonly Button _reanalyzeButton;
        private readonly Button _openNativeButton;
        private readonly Button _exportButton;
        private readonly Button _languageButton;
        private readonly FlowLayoutPanel _topPanel;
        private readonly Label _statusLabel;
        private readonly ProgressBar _progress;
        private readonly TabControl _tabs;
        private readonly RichTextBox _storyBox;
        private readonly RichTextBox _summaryBox;
        private readonly DataGridView _findingsGrid;
        private readonly DataGridView _incidentsGrid;
        private readonly DataGridView _timelineGrid;
        private readonly DataGridView _messagesGrid;
        private readonly DataGridView _gcsCommandsGrid;
        private readonly ComboBox _resultSelector;
        private readonly ComboBox _graphSelector;
        private readonly InvestigatorGraphControl _graphControl;
        private readonly InteractiveMapPanel _mapPanel;
        private readonly PilotCommandPanel _pilotCommandPanel;
        private readonly Label _gcsCommandStatusLabel;
        private readonly RichTextBox _infoBox;
        private readonly DataGridView _valuesGrid;
        private readonly ComboBox _valuesViewSelector;
        private readonly Label _valuesTimeLabel;
        private readonly BindingList<ValueRow> _valueRows = new BindingList<ValueRow>();
        private readonly BindingList<FindingRow> _findingRows = new BindingList<FindingRow>();
        private readonly BindingList<IncidentRow> _incidentRows = new BindingList<IncidentRow>();
        private readonly BindingList<TimelineRow> _timelineRows = new BindingList<TimelineRow>();
        private readonly BindingList<MessageRow> _messageRows = new BindingList<MessageRow>();
        private readonly BindingList<GcsCommandRow> _gcsCommandRows = new BindingList<GcsCommandRow>();
        private readonly List<LogAnalysisResult> _results = new List<LogAnalysisResult>();
        private bool _updatingGcsCommands;
        private bool _hebrew;
        private double _visualTimeMs;
        private string[] _selectedFiles = new string[0];

        public InvestigatorView()
        {
            Dock = DockStyle.Fill;
            BackColor = UiPalette.AppBackground;
            Font = new Font("Segoe UI", 9.0f);

            var headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 70;
            headerPanel.BackColor = UiPalette.Navy;

            var headerAccent = new Panel();
            headerAccent.Dock = DockStyle.Left;
            headerAccent.Width = 6;
            headerAccent.BackColor = UiPalette.Accent;

            var titleLabel = new Label();
            titleLabel.AutoSize = false;
            titleLabel.Location = new Point(24, 10);
            titleLabel.Size = new Size(560, 29);
            titleLabel.Text = "DRONE LOG INVESTIGATOR";
            titleLabel.Font = new Font("Segoe UI Semibold", 16.0f, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;

            var subtitleLabel = new Label();
            subtitleLabel.AutoSize = false;
            subtitleLabel.Location = new Point(26, 42);
            subtitleLabel.Size = new Size(720, 19);
            subtitleLabel.Text = "Clear, evidence-based ArduPilot flight investigation inside Mission Planner";
            subtitleLabel.Font = new Font("Segoe UI", 8.8f);
            subtitleLabel.ForeColor = UiPalette.Muted;

            var versionBadge = new Label();
            versionBadge.Dock = DockStyle.Right;
            versionBadge.Width = 150;
            versionBadge.Padding = new Padding(0, 0, 16, 0);
            versionBadge.TextAlign = ContentAlignment.MiddleRight;
            versionBadge.Text = "v0.5.7.1  HEBREW";
            versionBadge.Font = new Font("Segoe UI Semibold", 9.2f, FontStyle.Bold);
            versionBadge.ForeColor = UiPalette.Advisory;

            headerPanel.Controls.Add(versionBadge);
            headerPanel.Controls.Add(subtitleLabel);
            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(headerAccent);

            _topPanel = new FlowLayoutPanel();
            _topPanel.Dock = DockStyle.Top;
            _topPanel.Height = 52;
            _topPanel.Padding = new Padding(12, 8, 12, 6);
            _topPanel.WrapContents = false;
            _topPanel.BackColor = UiPalette.Surface;
            _topPanel.BorderStyle = BorderStyle.None;

            _openButton = NewPrimaryButton("Open logs", OpenButton_Click);
            _reanalyzeButton = NewButton("Analyze again", ReanalyzeButton_Click);
            _openNativeButton = NewButton("Open raw log", OpenNativeButton_Click);
            _exportButton = NewButton("Export report", ExportButton_Click);
            _languageButton = NewButton("עברית", LanguageButton_Click);
            _languageButton.Width = 66;

            _reanalyzeButton.Enabled = false;
            _openNativeButton.Enabled = false;
            _exportButton.Enabled = false;

            _statusLabel = new Label();
            _statusLabel.AutoSize = false;
            _statusLabel.Width = 420;
            _statusLabel.Height = 28;
            _statusLabel.TextAlign = ContentAlignment.MiddleLeft;
            _statusLabel.ForeColor = UiPalette.Text;
            _statusLabel.Font = new Font("Segoe UI", 8.8f);
            _statusLabel.Text = "Ready — open one or more ArduPilot logs to begin the investigation.";

            _progress = new ProgressBar();
            _progress.Width = 125;
            _progress.Height = 22;
            _progress.Style = ProgressBarStyle.Continuous;
            _progress.BackColor = UiPalette.SurfaceAlt;
            _progress.ForeColor = UiPalette.Accent;

            _resultSelector = new ComboBox();
            _resultSelector.Width = 270;
            _resultSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            UiPalette.StyleCombo(_resultSelector);
            _resultSelector.SelectedIndexChanged += ResultSelector_SelectedIndexChanged;

            _topPanel.Controls.Add(_openButton);
            _topPanel.Controls.Add(_reanalyzeButton);
            _topPanel.Controls.Add(_openNativeButton);
            _topPanel.Controls.Add(_exportButton);
            _topPanel.Controls.Add(_languageButton);
            _topPanel.Controls.Add(NewLabel("Selected log"));
            _topPanel.Controls.Add(_resultSelector);
            _topPanel.Controls.Add(_progress);
            _topPanel.Controls.Add(_statusLabel);

            _tabs = new TabControl();
            _tabs.Dock = DockStyle.Fill;
            _tabs.Font = new Font("Segoe UI Semibold", 9.1f, FontStyle.Bold);
            _tabs.Padding = new Point(14, 6);
            _tabs.SizeMode = TabSizeMode.Fixed;
            _tabs.ItemSize = new Size(142, 34);
            _tabs.DrawMode = TabDrawMode.OwnerDrawFixed;
            _tabs.DrawItem += Tabs_DrawItem;
            _tabs.SelectedIndexChanged += Tabs_SelectedIndexChanged;

            _storyBox = new RichTextBox();
            _storyBox.ReadOnly = true;
            _storyBox.ScrollBars = RichTextBoxScrollBars.Both;
            _storyBox.WordWrap = true;
            _storyBox.DetectUrls = false;
            _storyBox.Dock = DockStyle.Fill;
            _storyBox.Font = new Font("Segoe UI", 10.2f);
            _storyBox.BackColor = UiPalette.Surface;
            _storyBox.ForeColor = UiPalette.Text;
            _storyBox.BorderStyle = BorderStyle.None;

            _summaryBox = new RichTextBox();
            _summaryBox.ReadOnly = true;
            _summaryBox.ScrollBars = RichTextBoxScrollBars.Both;
            _summaryBox.WordWrap = false;
            _summaryBox.DetectUrls = false;
            _summaryBox.Dock = DockStyle.Fill;
            _summaryBox.Font = new Font(FontFamily.GenericMonospace, 9.3f);
            _summaryBox.BackColor = UiPalette.Surface;
            _summaryBox.ForeColor = UiPalette.Text;
            _summaryBox.BorderStyle = BorderStyle.None;

            _findingsGrid = NewGrid();
            _findingsGrid.AutoGenerateColumns = false;
            _findingsGrid.DataSource = _findingRows;
            AddFindingColumns(_findingsGrid);
            _findingsGrid.CellDoubleClick += FindingsGrid_CellDoubleClick;
            _findingsGrid.SelectionChanged += GridSelectionChanged;

            _incidentsGrid = NewGrid();
            _incidentsGrid.AutoGenerateColumns = false;
            _incidentsGrid.DataSource = _incidentRows;
            AddIncidentColumns(_incidentsGrid);
            _incidentsGrid.CellDoubleClick += IncidentsGrid_CellDoubleClick;
            _incidentsGrid.SelectionChanged += GridSelectionChanged;

            _timelineGrid = NewGrid();
            _timelineGrid.AutoGenerateColumns = false;
            _timelineGrid.DataSource = _timelineRows;
            AddTimelineColumns(_timelineGrid);
            _timelineGrid.CellDoubleClick += TimelineGrid_CellDoubleClick;
            _timelineGrid.SelectionChanged += GridSelectionChanged;

            _messagesGrid = NewGrid();
            _messagesGrid.AutoGenerateColumns = false;
            _messagesGrid.DataSource = _messageRows;
            AddMessageColumns(_messagesGrid);

            _gcsCommandsGrid = NewGrid();
            _gcsCommandsGrid.AutoGenerateColumns = false;
            _gcsCommandsGrid.DataSource = _gcsCommandRows;
            AddGcsCommandColumns(_gcsCommandsGrid);
            _gcsCommandsGrid.SelectionChanged += GcsCommandsGrid_SelectionChanged;
            _gcsCommandsGrid.CellDoubleClick += GcsCommandsGrid_CellDoubleClick;

            _pilotCommandPanel = new PilotCommandPanel();
            _pilotCommandPanel.Dock = DockStyle.Top;
            _pilotCommandPanel.Height = 330;

            _gcsCommandStatusLabel = new Label();
            _gcsCommandStatusLabel.Dock = DockStyle.Top;
            _gcsCommandStatusLabel.Height = 28;
            _gcsCommandStatusLabel.Padding = new Padding(14, 5, 0, 0);
            _gcsCommandStatusLabel.BackColor = UiPalette.Surface;
            _gcsCommandStatusLabel.ForeColor = UiPalette.Muted;
            _gcsCommandStatusLabel.Font = new Font("Segoe UI", 8.6f);
            _gcsCommandStatusLabel.Text = "No log selected.";

            var pilotCommandsHost = CreatePilotCommandsHost(_pilotCommandPanel, _gcsCommandsGrid, _gcsCommandStatusLabel);

            _graphSelector = new ComboBox();
            _graphSelector.Width = 230;
            _graphSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            UiPalette.StyleCombo(_graphSelector);
            _graphSelector.Items.AddRange(new object[]
            {
                "Battery voltage/current",
                "Vibration and clipping",
                "Attitude error",
                "Altitude and climb",
                "Motor outputs",
                "Pilot stick commands",
                "EKF health",
                "GPS quality"
            });
            _graphSelector.SelectedIndex = 0;
            _graphSelector.SelectedIndexChanged += GraphSelector_SelectedIndexChanged;

            var graphTopPanel = new FlowLayoutPanel();
            graphTopPanel.Dock = DockStyle.Top;
            graphTopPanel.Height = 47;
            graphTopPanel.Padding = new Padding(12, 7, 12, 6);
            graphTopPanel.WrapContents = false;
            graphTopPanel.BackColor = UiPalette.Surface;
            graphTopPanel.BorderStyle = BorderStyle.None;
            graphTopPanel.Controls.Add(NewLabel("Graph:"));
            graphTopPanel.Controls.Add(_graphSelector);
            graphTopPanel.Controls.Add(NewButton("Reset view", GraphResetButton_Click));
            graphTopPanel.Controls.Add(NewButton("Zoom +", GraphZoomInButton_Click));
            graphTopPanel.Controls.Add(NewButton("Zoom -", GraphZoomOutButton_Click));
            graphTopPanel.Controls.Add(NewButton("Show all", GraphShowAllButton_Click));
            graphTopPanel.Controls.Add(NewHintLabel("Mouse wheel: zoom   •   drag: pan   •   click: seek   •   legend: show/hide"));

            _graphControl = new InvestigatorGraphControl();
            _graphControl.Dock = DockStyle.Fill;
            _graphControl.TimeSelected += GraphControl_TimeSelected;

            var graphHost = new Panel();
            graphHost.Dock = DockStyle.Fill;
            graphHost.Controls.Add(_graphControl);
            graphHost.Controls.Add(graphTopPanel);

            _mapPanel = new InteractiveMapPanel();
            _mapPanel.Dock = DockStyle.Fill;
            _mapPanel.TimeChanged += MapPanel_TimeChanged;

            _valuesGrid = NewGrid();
            _valuesGrid.AutoGenerateColumns = false;
            _valuesGrid.DataSource = _valueRows;
            AddValueColumns(_valuesGrid);

            _valuesViewSelector = new ComboBox();
            _valuesViewSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            _valuesViewSelector.Width = 190;
            UiPalette.StyleCombo(_valuesViewSelector);
            _valuesViewSelector.Items.AddRange(new object[] { "Current snapshot", "Normalized signals" });
            _valuesViewSelector.SelectedIndex = 0;
            _valuesViewSelector.SelectedIndexChanged += ValuesViewSelector_SelectedIndexChanged;

            _valuesTimeLabel = new Label();
            _valuesTimeLabel.AutoSize = false;
            _valuesTimeLabel.Width = 360;
            _valuesTimeLabel.Height = 28;
            _valuesTimeLabel.TextAlign = ContentAlignment.MiddleLeft;
            _valuesTimeLabel.ForeColor = UiPalette.Advisory;
            _valuesTimeLabel.Font = new Font("Segoe UI Semibold", 8.8f, FontStyle.Bold);
            _valuesTimeLabel.Text = "No log selected.";

            var valuesTop = new FlowLayoutPanel();
            valuesTop.Dock = DockStyle.Top;
            valuesTop.Height = 48;
            valuesTop.Padding = new Padding(12, 7, 12, 6);
            valuesTop.WrapContents = false;
            valuesTop.BackColor = UiPalette.Surface;
            valuesTop.Controls.Add(NewLabel("Values view:"));
            valuesTop.Controls.Add(_valuesViewSelector);
            valuesTop.Controls.Add(_valuesTimeLabel);
            valuesTop.Controls.Add(NewHintLabel("Snapshot follows map / graph / incident time. Normalized signals use canonical engineering units without magnitude guessing."));

            var valuesHost = new Panel();
            valuesHost.Dock = DockStyle.Fill;
            valuesHost.BackColor = UiPalette.AppBackground;
            valuesHost.Controls.Add(_valuesGrid);
            valuesHost.Controls.Add(valuesTop);

            _infoBox = CreateInfoBox();

            _tabs.TabPages.Add(NewTab("Overview", _storyBox));
            _tabs.TabPages.Add(NewTab("Incidents", _incidentsGrid));
            _tabs.TabPages.Add(NewTab("Technical summary", _summaryBox));
            _tabs.TabPages.Add(NewTab("Findings", _findingsGrid));
            _tabs.TabPages.Add(NewTab("Timeline", _timelineGrid));
            _tabs.TabPages.Add(NewTab("Pilot & GCS", pilotCommandsHost));
            _tabs.TabPages.Add(NewTab("Graphs", graphHost));
            _tabs.TabPages.Add(NewTab("Interactive Map", _mapPanel));
            _tabs.TabPages.Add(NewTab("Values", valuesHost));
            _tabs.TabPages.Add(NewTab("Log data", _messagesGrid));
            _tabs.TabPages.Add(NewTab("Info", _infoBox));

            var creditLabel = new Label();
            creditLabel.Dock = DockStyle.Bottom;
            creditLabel.Height = 26;
            creditLabel.TextAlign = ContentAlignment.MiddleCenter;
            creditLabel.BackColor = UiPalette.Navy;
            creditLabel.ForeColor = UiPalette.Muted;
            creditLabel.Font = new Font("Segoe UI", 8.3f);
            creditLabel.Text = "Created by Nadav Golan-Yanay  •  © 2026 Nadav Golan-Yanay. All rights reserved.";

            Controls.Add(_tabs);
            Controls.Add(creditLabel);
            Controls.Add(_topPanel);
            Controls.Add(headerPanel);
            // Translate displayed grid text without changing the canonical investigation data.
            foreach (DataGridView localizedGrid in new[] { _findingsGrid, _incidentsGrid, _timelineGrid, _gcsCommandsGrid, _valuesGrid })
                localizedGrid.CellFormatting += LocalizedGrid_CellFormatting;

            // This embedded screen uses its own self-contained dark palette. Avoiding a recursive
            // theme pass keeps the header, tables, graph toolbar and map toolbar consistent.
            ApplyVisualPolish();
        }

        private void Tabs_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= _tabs.TabPages.Count)
                return;

            Rectangle bounds = e.Bounds;
            bool selected = e.Index == _tabs.SelectedIndex;
            Color background = selected ? UiPalette.SurfaceRaised : UiPalette.Navy;
            Color foreground = selected ? UiPalette.TextStrong : UiPalette.Muted;

            using (var backgroundBrush = new SolidBrush(background))
                e.Graphics.FillRectangle(backgroundBrush, bounds);

            if (selected)
            {
                using (var accentBrush = new SolidBrush(UiPalette.Accent))
                    e.Graphics.FillRectangle(accentBrush, bounds.Left + 8, bounds.Bottom - 3, bounds.Width - 16, 3);
            }

            TextRenderer.DrawText(e.Graphics, _tabs.TabPages[e.Index].Text, _tabs.Font,
                bounds, foreground, TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        private static Button NewButton(string text, EventHandler handler)
        {
            var button = new Button();
            button.Text = text;
            button.AutoSize = true;
            button.Click += handler;
            UiPalette.StyleButton(button, false);
            return button;
        }

        private void LocalizedGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (!_hebrew || e.Value == null) return;
            DataGridView grid = sender as DataGridView;
            if (grid == null || e.ColumnIndex < 0 || e.ColumnIndex >= grid.Columns.Count) return;
            string property = grid.Columns[e.ColumnIndex].DataPropertyName ?? string.Empty;
            string value = Convert.ToString(e.Value, CultureInfo.CurrentCulture);
            if (string.IsNullOrEmpty(value)) return;

            // Keep technical identifiers and raw numeric/time fields exactly as logged and LTR.
            bool technical = property == "File" || property == "Line" || property == "Time" || property == "Start" ||
                             property == "Value" || property == "Unit" || property == "SampleTime" || property == "Age" ||
                             property == "Source" || property == "Target" || property == "Method" || property == "Parameters" ||
                             property == "Position" || property == "Message" || property == "Fields" || property == "Count";
            if (technical)
            {
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                return;
            }

            string localized;
            if (property.IndexOf("Severity", StringComparison.OrdinalIgnoreCase) >= 0)
                localized = Localization.Severity(value);
            else if (property.IndexOf("Confidence", StringComparison.OrdinalIgnoreCase) >= 0)
                localized = Localization.Confidence(value);
            else if (property == "Phase")
                localized = Localization.Phase(value);
            else if (property == "Command" || property == "Commanded")
                localized = Localization.Command(value);
            else
                localized = Localization.Analysis(value);

            // RLM establishes a Hebrew paragraph base while preserving embedded GPS/EKF/MAVLink
            // identifiers and numbers in their normal LTR order.
            e.Value = "\u200F" + localized;
            e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            e.FormattingApplied = true;
        }

        private void LanguageButton_Click(object sender, EventArgs e)
        {
            _hebrew = !_hebrew;
            RefreshLocalizedReports();
            ApplyLanguage();
        }

        private void ApplyLanguage()
        {
            // Important: Hebrew is a presentation layer only. Do not mirror the whole WinForms
            // control tree. Mirroring TabControl/DataGridView/FlowLayoutPanel reverses tab order,
            // column order and toolbar geometry and also breaks code that relies on canonical UI
            // identity. Keep the physical UI layout identical to English and apply RTL only to
            // prose surfaces.
            TranslateControlTextRecursive(this);
            _languageButton.Text = _hebrew ? "English" : "עברית";

            _tabs.RightToLeft = RightToLeft.No;
            _tabs.RightToLeftLayout = false;
            _topPanel.FlowDirection = FlowDirection.LeftToRight;

            _resultSelector.RightToLeft = RightToLeft.No;
            _graphSelector.RightToLeft = RightToLeft.No;
            _valuesViewSelector.RightToLeft = RightToLeft.No;

            foreach (DataGridView grid in new[] { _findingsGrid, _incidentsGrid, _timelineGrid, _messagesGrid, _gcsCommandsGrid, _valuesGrid })
                ApplyGridLanguage(grid);

            RefreshSelectorLanguage();
            ApplyReportTextDirection(_storyBox);
            ApplyReportTextDirection(_summaryBox);

            _pilotCommandPanel.Hebrew = _hebrew;
            _statusLabel.TextAlign = _hebrew ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft;
            _valuesTimeLabel.TextAlign = _hebrew ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft;

            if (_results.Count > 0)
            {
                _statusLabel.Text = _hebrew
                    ? string.Format(CultureInfo.InvariantCulture, "הסתיים: {0} לוגים, {1} ממצאים.", _results.Count, _results.Sum(r => r.Findings.Count))
                    : string.Format(CultureInfo.InvariantCulture, "Finished: {0} log(s), {1} finding(s).", _results.Count, _results.Sum(r => r.Findings.Count));
                PopulateGcsCommands(GetVisualizedResult());
                UpdateValues(_visualTimeMs);
            }
            else if (_selectedFiles == null || _selectedFiles.Length == 0)
            {
                _statusLabel.Text = Localization.Ui("Ready — open one or more ArduPilot logs to begin the investigation.", _hebrew);
                _valuesTimeLabel.Text = Localization.Ui("No log selected.", _hebrew);
                _gcsCommandStatusLabel.Text = Localization.Ui("No log selected.", _hebrew);
            }

            RefreshInfoLanguage();
            _tabs.Invalidate();
            Invalidate(true);
        }

        private void TranslateControlTextRecursive(Control control)
        {
            if (control == null) return;

            // Translate labels without changing the control tree direction. Report boxes and
            // Info are populated explicitly so paragraph direction can be handled separately.
            if (control != _languageButton && control != _storyBox && control != _summaryBox && control != _infoBox)
                control.Text = Localization.Ui(control.Text, _hebrew);

            DataGridView grid = control as DataGridView;
            if (grid != null)
            {
                foreach (DataGridViewColumn column in grid.Columns)
                    column.HeaderText = Localization.Ui(column.HeaderText, _hebrew);
            }

            foreach (Control child in control.Controls)
                TranslateControlTextRecursive(child);
        }

        private void ApplyReportTextDirection(RichTextBox box)
        {
            if (box == null) return;
            box.RightToLeft = _hebrew ? RightToLeft.Yes : RightToLeft.No;
            box.SelectAll();
            box.SelectionAlignment = _hebrew ? HorizontalAlignment.Right : HorizontalAlignment.Left;
            box.SelectionLength = 0;
            box.SelectionStart = 0;
        }

        private void ApplyGridLanguage(DataGridView grid)
        {
            if (grid == null) return;

            // Keep the canonical English column order in both languages. Hebrew prose is
            // right-aligned inside the existing columns; technical/time/numeric fields remain
            // left/center aligned so mixed Hebrew + MAVLink/DataFlash content stays readable.
            grid.RightToLeft = RightToLeft.No;
            grid.ColumnHeadersDefaultCellStyle.Alignment = _hebrew
                ? DataGridViewContentAlignment.MiddleRight
                : DataGridViewContentAlignment.MiddleLeft;

            foreach (DataGridViewColumn column in grid.Columns)
            {
                string property = column.DataPropertyName ?? string.Empty;
                bool technical = property == "File" || property == "Line" || property == "Time" || property == "Start" ||
                                 property == "Value" || property == "Unit" || property == "SampleTime" || property == "Age" ||
                                 property == "Source" || property == "Target" || property == "Method" || property == "Parameters" ||
                                 property == "Position" || property == "Message" || property == "Fields" || property == "Count";
                column.DefaultCellStyle.Alignment = technical
                    ? DataGridViewContentAlignment.MiddleLeft
                    : (_hebrew ? DataGridViewContentAlignment.MiddleRight : DataGridViewContentAlignment.MiddleLeft);
            }
        }

        private void RefreshSelectorLanguage()
        {
            int graphIndex = _graphSelector.SelectedIndex;
            int valuesIndex = _valuesViewSelector.SelectedIndex;

            string[] graphEnglish =
            {
                "Battery voltage/current",
                "Vibration and clipping",
                "Attitude error",
                "Altitude and climb",
                "Motor outputs",
                "Pilot stick commands",
                "EKF health",
                "GPS quality"
            };
            string[] graphHebrew =
            {
                "מתח / זרם סוללה",
                "רעידות ו-clipping",
                "שגיאת מצב זוויתי",
                "גובה וקצב אנכי",
                "פקודות מנועים",
                "קלטי סטיקים",
                "מצב EKF",
                "איכות GPS"
            };

            _graphSelector.BeginUpdate();
            try
            {
                _graphSelector.Items.Clear();
                _graphSelector.Items.AddRange((_hebrew ? graphHebrew : graphEnglish).Cast<object>().ToArray());
                if (_graphSelector.Items.Count > 0)
                    _graphSelector.SelectedIndex = Math.Max(0, Math.Min(graphIndex < 0 ? 0 : graphIndex, _graphSelector.Items.Count - 1));
            }
            finally { _graphSelector.EndUpdate(); }

            _valuesViewSelector.BeginUpdate();
            try
            {
                _valuesViewSelector.Items.Clear();
                _valuesViewSelector.Items.AddRange(_hebrew
                    ? new object[] { "תמונת מצב נוכחית", "אותות מנורמלים" }
                    : new object[] { "Current snapshot", "Normalized signals" });
                if (_valuesViewSelector.Items.Count > 0)
                    _valuesViewSelector.SelectedIndex = Math.Max(0, Math.Min(valuesIndex < 0 ? 0 : valuesIndex, _valuesViewSelector.Items.Count - 1));
            }
            finally { _valuesViewSelector.EndUpdate(); }
        }

        private static string CanonicalGraphName(int index)
        {
            switch (index)
            {
                case 0: return "Battery voltage/current";
                case 1: return "Vibration and clipping";
                case 2: return "Attitude error";
                case 3: return "Altitude and climb";
                case 4: return "Motor outputs";
                case 5: return "Pilot stick commands";
                case 6: return "EKF health";
                case 7: return "GPS quality";
                default: return "Battery voltage/current";
            }
        }

        private bool IsSelectedTab(string canonicalTitle)
        {
            if (_tabs.SelectedTab == null) return false;
            string tag = Convert.ToString(_tabs.SelectedTab.Tag, CultureInfo.InvariantCulture);
            return string.Equals(tag, canonicalTitle, StringComparison.Ordinal);
        }

        private void RefreshInfoLanguage()
        {
            if (_hebrew)
            {
                _infoBox.Clear();
                _infoBox.RightToLeft = RightToLeft.Yes;
                _infoBox.SelectionAlignment = HorizontalAlignment.Right;
                _infoBox.Font = new Font("Segoe UI", 10.0f);
                _infoBox.ForeColor = UiPalette.Text;
                _infoBox.Text =
                    "חוקר לוגי טיסה v0.5.7.1 — מדריך מקוצר\r\n\r\n" +
                    "מטרה\r\n" +
                    "הכלי מנתח לוגי ArduPilot בתוך Mission Planner ומציג ממצאים, אירועים מתואמים, ציר זמן, פקודות טייס/GCS, גרפים, מפה וערכים מנורמלים.\r\n\r\n" +
                    "עקרונות החקירה\r\n" +
                    "• המסקנות מבוססות על ראיות מתועדות ומתאם בזמן.\r\n" +
                    "• הכלי מפריד בין עובדות המתועדות בלוג, מסקנות הנתמכות בראיות, סיבות אפשריות ומה שלא ניתן לקבוע.\r\n" +
                    "• פלט RCOU הוא פלט פקוד; הוא אינו מוכיח RPM או דחף ללא טלמטריית ESC/RPM.\r\n" +
                    "• לוג חלקי או סיום חריג אינם מוכיחים אובדן מתח.\r\n\r\n" +
                    "לשוניות\r\n" +
                    "• סקירה — סיפור חקירה מסודר ומבוסס ראיות.\r\n" +
                    "• אירועים חריגים — אירועים מורכבים עם חומרה נפרדת ורמת ביטחון נפרדת בזיהוי האירוע ובסיבה הראשונית.\r\n" +
                    "• ממצאים / ציר זמן — ממצאים מפורטים ורשומות אירוע.\r\n" +
                    "• טייס ו-GCS — RCIN, RC override ופקודות MAVLink מתועדות.\r\n" +
                    "• גרפים / מפה — חקירה מסונכרנת בזמן.\r\n" +
                    "• ערכים — תמונת מצב ונתונים מנורמלים בזמן הסמן.\r\n\r\n" +
                    "שפה\r\n" +
                    "כפתור English / עברית מחליף את שפת הממשק והדוח. שמות הודעות DataFlash, פרמטרים ופקודות MAVLink נשארים באנגלית כדי לשמור על התאמה ללוג המקורי.\r\n\r\n" +
                    "מגבלה\r\n" +
                    "הממצאים האוטומטיים הם כלי עזר מבוסס ראיות, לא הוכחה סופית. יש לאמת מסקנות מול הרשומות הגולמיות, הכלי הפיזי, היסטוריית התצורה וטיסה תקינה להשוואה.\r\n\r\n" +
                    "נוצר על ידי Nadav Golan-Yanay\r\n© 2026 Nadav Golan-Yanay. כל הזכויות שמורות.";
                _infoBox.SelectionStart = 0;
                _infoBox.SelectionLength = 0;
            }
            else
            {
                using (RichTextBox fresh = CreateInfoBox())
                    _infoBox.Rtf = fresh.Rtf;
                _infoBox.RightToLeft = RightToLeft.No;
                _infoBox.SelectionAlignment = HorizontalAlignment.Left;
            }
        }

        private void RefreshLocalizedReports()
        {
            if (_results.Count == 0) return;
            _summaryBox.Text = ReportBuilder.BuildPlainText(_results, _hebrew);
            _storyBox.Text = ReportBuilder.BuildNarrative(_results, _hebrew);
            ApplyReportTextDirection(_summaryBox);
            ApplyReportTextDirection(_storyBox);
        }

        public void Activate()
        {
            Dock = DockStyle.Fill;
            Visible = true;
            BringToFront();
            Invalidate(true);
        }

        public void Deactivate()
        {
            if (_mapPanel != null)
                _mapPanel.PausePlayback();
        }

        private static Button NewPrimaryButton(string text, EventHandler handler)
        {
            var button = new Button();
            button.Text = text;
            button.AutoSize = true;
            button.Click += handler;
            UiPalette.StyleButton(button, true);
            return button;
        }

        private static Label NewLabel(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = UiPalette.Text,
                Font = new Font("Segoe UI Semibold", 8.8f, FontStyle.Bold),
                Padding = new Padding(8, 7, 2, 0)
            };
        }

        private static Label NewHintLabel(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = UiPalette.Muted,
                Font = new Font("Segoe UI", 8.3f),
                Padding = new Padding(10, 8, 2, 0)
            };
        }

        private static Label NewDockLabel(string text, DockStyle dock, int width)
        {
            return new Label
            {
                Text = text,
                Dock = dock,
                Width = width,
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private static TabPage NewTab(string title, Control control)
        {
            var tab = new TabPage(title);
            tab.Name = title;
            tab.Tag = title;
            tab.BackColor = UiPalette.AppBackground;
            tab.ForeColor = UiPalette.Text;
            tab.Padding = new Padding(10);
            var host = new Panel();
            host.Dock = DockStyle.Fill;
            host.BackColor = UiPalette.AppBackground;
            host.Padding = new Padding(2);
            control.Dock = DockStyle.Fill;
            host.Controls.Add(control);
            tab.Controls.Add(host);
            return tab;
        }

        private static Control CreatePilotCommandsHost(PilotCommandPanel pilotPanel, DataGridView commandGrid, Label statusLabel)
        {
            var host = new Panel();
            host.Dock = DockStyle.Fill;
            host.BackColor = UiPalette.AppBackground;

            var header = new Panel();
            header.Dock = DockStyle.Top;
            header.Height = 62;
            header.BackColor = UiPalette.Surface;
            header.Padding = new Padding(16, 8, 16, 6);

            var title = new Label();
            title.Dock = DockStyle.Top;
            title.Height = 23;
            title.Text = "PILOT INPUTS AND GCS / MAVLINK COMMANDS";
            title.Font = new Font("Segoe UI Semibold", 11.5f, FontStyle.Bold);
            title.ForeColor = UiPalette.TextStrong;

            var note = new Label();
            note.Dock = DockStyle.Fill;
            note.Text = "RCIN shows the control inputs seen by the autopilot. RCI2 identifies MAVLink RC override channels. MAVC lists executed MAVLink commands when the firmware logged them.";
            note.Font = new Font("Segoe UI", 8.7f);
            note.ForeColor = UiPalette.Muted;

            header.Controls.Add(note);
            header.Controls.Add(title);

            var gridTitle = new Label();
            gridTitle.Dock = DockStyle.Top;
            gridTitle.Height = 34;
            gridTitle.Padding = new Padding(14, 8, 0, 0);
            gridTitle.Text = "GROUND-STATION / MAVLINK COMMAND HISTORY";
            gridTitle.Font = new Font("Segoe UI Semibold", 9.3f, FontStyle.Bold);
            gridTitle.ForeColor = UiPalette.Advisory;
            gridTitle.BackColor = UiPalette.SurfaceAlt;

            commandGrid.Dock = DockStyle.Fill;
            host.Controls.Add(commandGrid);
            host.Controls.Add(statusLabel);
            host.Controls.Add(gridTitle);
            host.Controls.Add(pilotPanel);
            host.Controls.Add(header);
            return host;
        }

        private static void AddIncidentColumns(DataGridView grid)
        {
            grid.Columns.Add(NewTextColumn("Severity", "Severity", 84));
            grid.Columns.Add(NewTextColumn("EventConfidence", "Event confidence", 108));
            grid.Columns.Add(NewTextColumn("CauseConfidence", "Cause confidence", 108));
            grid.Columns.Add(NewTextColumn("File", "File", 145));
            grid.Columns.Add(NewTextColumn("Time", "Start", 92));
            grid.Columns.Add(NewTextColumn("Phase", "Phase", 112));
            grid.Columns.Add(NewTextColumn("Category", "Category", 105));
            grid.Columns.Add(NewTextColumn("Incident", "Principal incident", 250));
            grid.Columns.Add(NewTextColumn("Command", "Commanded?", 150));
            grid.Columns.Add(NewTextColumn("Evidence", "Correlated evidence", 520));
        }

        private static void AddGcsCommandColumns(DataGridView grid)
        {
            grid.Columns.Add(NewTextColumn("Time", "Time", 92));
            grid.Columns.Add(NewTextColumn("Phase", "Phase", 95));
            grid.Columns.Add(NewTextColumn("Source", "Source", 92));
            grid.Columns.Add(NewTextColumn("Target", "Target", 92));
            grid.Columns.Add(NewTextColumn("Command", "Command", 230));
            grid.Columns.Add(NewTextColumn("Result", "Result", 135));
            grid.Columns.Add(NewTextColumn("Method", "Method", 112));
            grid.Columns.Add(NewTextColumn("Parameters", "Parameters", 340));
            grid.Columns.Add(NewTextColumn("Position", "Position / XYZ", 230));
            grid.Columns.Add(NewTextColumn("Line", "Line", 68));
        }

        private static void AddValueColumns(DataGridView grid)
        {
            grid.Columns.Add(NewTextColumn("Category", "Category", 115));
            grid.Columns.Add(NewTextColumn("Signal", "Signal / value", 230));
            grid.Columns.Add(NewTextColumn("Value", "Value", 125));
            grid.Columns.Add(NewTextColumn("Unit", "Unit", 80));
            grid.Columns.Add(NewTextColumn("Source", "Source", 115));
            grid.Columns.Add(NewTextColumn("SampleTime", "Sample time", 105));
            grid.Columns.Add(NewTextColumn("Age", "Age", 80));
            grid.Columns.Add(NewTextColumn("Phase", "Phase", 120));
            grid.Columns.Add(NewTextColumn("Notes", "Interpretation / normalization", 420));
        }

        private static RichTextBox CreateInfoBox()
        {
            var box = new RichTextBox();
            box.Dock = DockStyle.Fill;
            box.ReadOnly = true;
            box.WordWrap = true;
            box.DetectUrls = true;
            box.ScrollBars = RichTextBoxScrollBars.Vertical;
            box.BorderStyle = BorderStyle.None;
            box.BackColor = UiPalette.Surface;
            box.ForeColor = UiPalette.Text;
            box.Font = new Font("Segoe UI", 10.0f);
            box.Margin = new Padding(0);
            box.LinkClicked += delegate(object sender, LinkClickedEventArgs e)
            {
                try
                {
                    Process.Start(e.LinkText);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not open the link.\r\n\r\n" + ex.Message,
                        "Open link", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            AppendInfoTitle(box, "DRONE LOG INVESTIGATOR v0.5.7.1 HEBREW POLISH — INFORMATION & QUICK GUIDE");
            AppendInfoText(box,
                "This page explains what the plugin does, how to investigate a flight, where each feature is located, and who contributed to the project.\n\n");

            AppendInfoHeading(box, "1. Information about the plugin");
            AppendInfoBullet(box, "Purpose", "Turn ArduPilot DataFlash .BIN/.LOG files into a clear, evidence-based investigation in Mission Planner’s main workspace.");
            AppendInfoBullet(box, "How it works", "The plugin uses deterministic checks and correlations across events, battery, vibration, GPS, attitude, altitude, motor outputs, EKF and logging health.");
            AppendInfoBullet(box, "Privacy", "Analysis runs locally on the computer. The log is not uploaded by this plugin.");
            AppendInfoBullet(box, "Output", "Plain-language overview, technical findings, synchronized timeline, interactive graphs, flight replay map and an exportable HTML report—all retained when switching between Mission Planner screens.");
            AppendInfoBullet(box, "Important limitation", "An automatic finding is evidence, not proof of root cause. Always verify the raw records and compare with a healthy flight of the same aircraft.");

            AppendInfoHeading(box, "2. Basic skills for investigating a flight");
            AppendInfoNumbered(box, 1, "Start with the question", "Define the reported symptom and the approximate time: mode change, descent, vibration, GPS loss, rough landing, power problem or loss of control.");
            AppendInfoNumbered(box, 2, "Build the timeline", "Identify arming, takeoff, mode changes, the reported event, recovery actions, landing and disarming.");
            AppendInfoNumbered(box, 3, "Look before the event", "The cause usually begins before the visible symptom. Review at least 10–30 seconds before the event.");
            AppendInfoNumbered(box, 4, "Correlate independent evidence", "Do not rely on one signal. Compare pilot commands, desired versus actual attitude, motor outputs, battery, vibration, GPS and EKF at the same time.");
            AppendInfoNumbered(box, 5, "Separate cause from consequence", "For example, high motor output may be the response to a tilt or wind—not the original fault.");
            AppendInfoNumbered(box, 6, "Use flight phase and mode", "The same value can be normal during takeoff but abnormal during stable hover. Interpret it in context.");
            AppendInfoNumbered(box, 7, "Verify in the raw log", "Double-click a finding or timeline row, then use Mission Planner Log Browse to inspect the original message and neighboring data.");
            AppendInfoNumbered(box, 8, "Compare with a known-good log", "Aircraft-specific baselines are often more useful than generic thresholds.");
            AppendInfoNumbered(box, 9, "State uncertainty", "Record the most likely explanation, alternatives, supporting evidence, conflicting evidence and confidence level.");

            AppendInfoHeading(box, "3. App map — where everything is and what it does");
            AppendInfoSubheading(box, "Top command bar");
            AppendInfoBullet(box, "Open logs", "Select one or several ArduPilot .BIN/.LOG files.");
            AppendInfoBullet(box, "Analyze again", "Re-run the investigation on the currently selected files.");
            AppendInfoBullet(box, "Open raw log", "Open the selected evidence in Mission Planner Log Browse. The relevant line number is copied for Ctrl+G.");
            AppendInfoBullet(box, "Export report", "Save a standalone HTML investigation report.");
            AppendInfoBullet(box, "Selected log", "Choose which loaded log drives the overview, graphs and map.");

            AppendInfoSubheading(box, "Tabs");
            AppendInfoBullet(box, "Overview", "A plain-language account of what happened and the most important findings.");
            AppendInfoBullet(box, "Incidents", "Correlation-based principal events with separate event confidence, root-cause confidence, command classification and supporting evidence.");
            AppendInfoBullet(box, "Technical summary", "Detailed file statistics, findings and event records.");
            AppendInfoBullet(box, "Findings", "Sortable evidence table with severity, confidence, time, phase, subsystem and recommended next action.");
            AppendInfoBullet(box, "Timeline", "Chronological arming, mode, event, error and firmware-message records.");
            AppendInfoBullet(box, "Pilot & GCS", "Replays roll, pitch, throttle and yaw inputs from RCIN, indicates MAVLink RC overrides from RCI2, and lists executed MAVLink commands from MAVC.");
            AppendInfoBullet(box, "v0.5.3 foundation", "Adds a unified resolved flight-state timeline, firmware/vehicle profiling, parameter-aware sensor activity, reasoned missing-data classification, and a normalized engineering-unit data layer.");
            AppendInfoBullet(box, "v0.5.5.1 evidence-led reporting + correctness checks", "Adds selected-primary GPS metrics, coordinate-jump filtering, one log-relative time origin, incident limitation propagation and armed-ground altitude-reference reporting while retaining the v0.5.5 investigation structure.");
            AppendInfoBullet(box, "v0.5.6 investigation tools", "Adds selected time ranges on the map, receiver-specific and all-GPS track views with type labels, mission waypoints, mode/reference overlays, synchronized values, and a visible normalized-data inspector.");
            AppendInfoBullet(box, "v0.5.7.1 Hebrew polish", "Keeps the English/Hebrew switch but uses selective RTL only for Hebrew prose. Tab order, grid column order, controls and technical identifiers remain stable in both languages.");
            AppendInfoBullet(box, "Graphs", "Interactive subsystem plots. Wheel to zoom, drag to pan, hover for values, click to seek, click a legend item to show/hide a series.");
            AppendInfoBullet(box, "Interactive Map", "Flight replay. Play/reverse, step one second, scrub time, pan/zoom, click the track or event marker, follow the vehicle, show events/future path and fit the complete track.");
            AppendInfoBullet(box, "Log data", "Inventory of message types, record counts and available fields.");
            AppendInfoBullet(box, "Info", "This help, contact and attribution page.");

            AppendInfoSubheading(box, "Severity and confidence");
            AppendInfoBullet(box, "Critical", "Strong evidence of a condition that may explain an unsafe event or loss of control.");
            AppendInfoBullet(box, "Warning", "Abnormal behavior requiring investigation.");
            AppendInfoBullet(box, "Advisory", "Useful observation, missing data or possible improvement.");
            AppendInfoBullet(box, "Confidence", "How strongly the available evidence supports the finding—not how severe it is.");

            AppendInfoHeading(box, "4. About Nadav Golan-Yanay");
            AppendInfoText(box, "Creator and project owner: Nadav Golan-Yanay\n");
            AppendInfoText(box, "Email: nadavgy1@gmail.com\n");
            AppendInfoText(box, "Email link: mailto:nadavgy1@gmail.com\n");
            AppendInfoText(box, "GitHub: https://github.com/nadav-golan-yanay\n");

            AppendInfoHeading(box, "5. Contributors and resources");
            AppendInfoBullet(box, "Nadav Golan-Yanay", "Creator, product definition, investigation requirements, testing and project ownership.");
            AppendInfoBullet(box, "OpenAI ChatGPT — GPT-5.6 Thinking", "Development assistance including architecture, code generation, debugging, analyzer design, user-interface design and documentation.");
            AppendInfoBullet(box, "ArduPilot and Mission Planner contributors", "Mission Planner platform, plugin architecture, DataFlash formats, DFLogBuffer parser, Log Browse and ecosystem documentation.");
            AppendInfoBullet(box, "Microsoft .NET / Windows Forms", "Desktop user-interface and drawing framework used by the plugin.");
            AppendInfoBullet(box, "UAVLogViewer", "Interaction reference for time-linked flight replay, maps and graph exploration. The plugin remains a native Mission Planner implementation.");

            AppendInfoSubheading(box, "Project resources");
            AppendInfoText(box, "Mission Planner source: https://github.com/ArduPilot/MissionPlanner\n");
            AppendInfoText(box, "ArduPilot Mission Planner documentation: https://ardupilot.org/planner/\n");
            AppendInfoText(box, "UAVLogViewer documentation: https://ardupilot.org/sub/docs/common-uavlogviewer.html\n");
            AppendInfoText(box, "OpenAI: https://openai.com/\n\n");
            AppendInfoText(box, "The names of third-party projects are included for attribution and technical context; this does not imply endorsement.\n\n");
            AppendInfoText(box, "Command-data limitation: MAVC contains commands the autopilot executed and logged. It may not contain every telemetry message, parameter write, manual-control packet or action performed by a ground station.\n\n");
            AppendInfoText(box, "Created by Nadav Golan-Yanay\n© 2026 Nadav Golan-Yanay. All rights reserved.\n");

            box.SelectionStart = 0;
            box.SelectionLength = 0;
            return box;
        }

        private static void AppendInfoTitle(RichTextBox box, string text)
        {
            AppendFormattedInfo(box, text + "\n", new Font("Segoe UI Semibold", 17.0f, FontStyle.Bold), UiPalette.TextStrong);
            AppendFormattedInfo(box, new string('─', 72) + "\n", new Font("Segoe UI", 8.0f), UiPalette.Accent);
        }

        private static void AppendInfoHeading(RichTextBox box, string text)
        {
            AppendFormattedInfo(box, "\n" + text + "\n", new Font("Segoe UI Semibold", 13.0f, FontStyle.Bold), UiPalette.Advisory);
        }

        private static void AppendInfoSubheading(RichTextBox box, string text)
        {
            AppendFormattedInfo(box, "\n" + text + "\n", new Font("Segoe UI Semibold", 10.5f, FontStyle.Bold), UiPalette.TextStrong);
        }

        private static void AppendInfoBullet(RichTextBox box, string label, string text)
        {
            AppendFormattedInfo(box, "• " + label + ": ", new Font("Segoe UI Semibold", 10.0f, FontStyle.Bold), UiPalette.TextStrong);
            AppendInfoText(box, text + "\n");
        }

        private static void AppendInfoNumbered(RichTextBox box, int number, string label, string text)
        {
            AppendFormattedInfo(box, number.ToString(CultureInfo.InvariantCulture) + ". " + label + ": ",
                new Font("Segoe UI Semibold", 10.0f, FontStyle.Bold), UiPalette.TextStrong);
            AppendInfoText(box, text + "\n");
        }

        private static void AppendInfoText(RichTextBox box, string text)
        {
            AppendFormattedInfo(box, text, new Font("Segoe UI", 10.0f), UiPalette.Text);
        }

        private static void AppendFormattedInfo(RichTextBox box, string text, Font font, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;
            box.SelectionFont = font;
            box.SelectionColor = color;
            box.AppendText(text);
        }

        private static DataGridView NewGrid()
        {
            var grid = new DataGridView();
            grid.Dock = DockStyle.Fill;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToOrderColumns = true;
            grid.AllowUserToResizeRows = false;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.RowHeadersVisible = false;
            grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            grid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            grid.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            grid.BackgroundColor = UiPalette.Surface;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = UiPalette.Grid;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersHeight = 38;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersDefaultCellStyle.BackColor = UiPalette.NavyLight;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.0f, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = UiPalette.NavyLight;
            grid.DefaultCellStyle.BackColor = UiPalette.Surface;
            grid.DefaultCellStyle.ForeColor = UiPalette.Text;
            grid.DefaultCellStyle.SelectionBackColor = UiPalette.Selection;
            grid.DefaultCellStyle.SelectionForeColor = UiPalette.TextStrong;
            grid.DefaultCellStyle.Padding = new Padding(5, 4, 5, 4);
            grid.AlternatingRowsDefaultCellStyle.BackColor = UiPalette.SurfaceAlt;
            grid.DefaultCellStyle.NullValue = "—";
            return grid;
        }

        private void ApplyVisualPolish()
        {
            BackColor = UiPalette.AppBackground;
            _tabs.BackColor = UiPalette.AppBackground;
            _storyBox.BackColor = UiPalette.Surface;
            _storyBox.ForeColor = UiPalette.Text;
            _summaryBox.BackColor = UiPalette.Surface;
            _summaryBox.ForeColor = UiPalette.Text;
            UiPalette.StyleCombo(_resultSelector);
            UiPalette.StyleCombo(_graphSelector);
            UiPalette.StyleButton(_openButton, true);
            UiPalette.StyleButton(_reanalyzeButton, false);
            UiPalette.StyleButton(_openNativeButton, false);
            UiPalette.StyleButton(_exportButton, false);
            _tabs.Invalidate();
        }

        private static void AddFindingColumns(DataGridView grid)
        {
            grid.Columns.Add(NewTextColumn("Severity", "Severity", 78));
            grid.Columns.Add(NewTextColumn("Confidence", "Confidence", 82));
            grid.Columns.Add(NewTextColumn("File", "File", 145));
            grid.Columns.Add(NewTextColumn("Time", "Time", 80));
            grid.Columns.Add(NewTextColumn("Phase", "Phase", 85));
            grid.Columns.Add(NewTextColumn("Subsystem", "Subsystem", 95));
            grid.Columns.Add(NewTextColumn("Finding", "Finding", 280));
            grid.Columns.Add(NewTextColumn("Evidence", "Evidence", 430));
            grid.Columns.Add(NewTextColumn("Recommendation", "Recommendation", 430));
            grid.Columns.Add(NewTextColumn("Line", "Line", 65));
        }

        private static void AddTimelineColumns(DataGridView grid)
        {
            grid.Columns.Add(NewTextColumn("File", "File", 160));
            grid.Columns.Add(NewTextColumn("Time", "Time", 80));
            grid.Columns.Add(NewTextColumn("Phase", "Phase", 90));
            grid.Columns.Add(NewTextColumn("Type", "Type", 90));
            grid.Columns.Add(NewTextColumn("Text", "Text", 800));
            grid.Columns.Add(NewTextColumn("Line", "Line", 70));
        }

        private static void AddMessageColumns(DataGridView grid)
        {
            grid.Columns.Add(NewTextColumn("File", "File", 220));
            grid.Columns.Add(NewTextColumn("Message", "Message", 120));
            grid.Columns.Add(NewTextColumn("Count", "Count", 110));
            grid.Columns.Add(NewTextColumn("Fields", "Fields", 850));
        }

        private static DataGridViewTextBoxColumn NewTextColumn(string property, string header, int width)
        {
            var column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = property;
            column.HeaderText = header;
            column.Width = width;
            column.SortMode = DataGridViewColumnSortMode.Automatic;
            return column;
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "ArduPilot logs|*.bin;*.BIN;*.log;*.LOG|All files|*.*";
                dialog.Multiselect = true;
                dialog.Title = "Select ArduPilot dataflash logs";
                try
                {
                    if (!string.IsNullOrEmpty(Settings.Instance.LogDir) && Directory.Exists(Settings.Instance.LogDir))
                        dialog.InitialDirectory = Settings.Instance.LogDir;
                }
                catch
                {
                    // Optional convenience only.
                }

                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;

                _selectedFiles = dialog.FileNames;
                AnalyzeSelectedFiles();
            }
        }

        private void ReanalyzeButton_Click(object sender, EventArgs e)
        {
            AnalyzeSelectedFiles();
        }

        private async void AnalyzeSelectedFiles()
        {
            if (_selectedFiles == null || _selectedFiles.Length == 0)
                return;

            SetBusy(true, "Preparing analysis...");
            _results.Clear();
            _findingRows.Clear();
            _incidentRows.Clear();
            _timelineRows.Clear();
            _messageRows.Clear();
            _gcsCommandRows.Clear();
            _summaryBox.Clear();

            try
            {
                for (int i = 0; i < _selectedFiles.Length; i++)
                {
                    string file = _selectedFiles[i];
                    int index = i;
                    _statusLabel.Text = _hebrew
                        ? string.Format(CultureInfo.InvariantCulture, "מנתח {0} ({1}/{2})...", Path.GetFileName(file), i + 1, _selectedFiles.Length)
                        : string.Format(CultureInfo.InvariantCulture, "Analyzing {0} ({1}/{2})...", Path.GetFileName(file), i + 1, _selectedFiles.Length);
                    _progress.Style = ProgressBarStyle.Continuous;
                    _progress.Minimum = 0;
                    _progress.Maximum = Math.Max(1, _selectedFiles.Length);
                    _progress.Value = Math.Min(index, _progress.Maximum);

                    LogAnalysisResult result = await Task.Run(delegate { return LogAnalyzer.Analyze(file); });
                    _results.Add(result);
                    _progress.Value = Math.Min(i + 1, _progress.Maximum);
                }

                PopulateResults();
                _statusLabel.Text = _hebrew
                    ? string.Format(CultureInfo.InvariantCulture, "הסתיים: {0} לוגים, {1} ממצאים.", _results.Count, _results.Sum(r => r.Findings.Count))
                    : string.Format(CultureInfo.InvariantCulture, "Finished: {0} log(s), {1} finding(s).", _results.Count, _results.Sum(r => r.Findings.Count));
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), "Log Investigator error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _statusLabel.Text = Localization.Ui("Analysis stopped because of an error.", _hebrew);
            }
            finally
            {
                SetBusy(false, _statusLabel.Text);
            }
        }

        private void SetBusy(bool busy, string status)
        {
            _openButton.Enabled = !busy;
            _reanalyzeButton.Enabled = !busy && _selectedFiles.Length > 0;
            _openNativeButton.Enabled = !busy && _selectedFiles.Length > 0;
            _exportButton.Enabled = !busy && _results.Count > 0;
            _statusLabel.Text = _hebrew ? Localization.Analysis(status) : status;
            UseWaitCursor = busy;
            if (busy)
                _progress.Style = ProgressBarStyle.Marquee;
            else
                _progress.Style = ProgressBarStyle.Continuous;
            _progress.BackColor = UiPalette.SurfaceAlt;
            _progress.ForeColor = UiPalette.Accent;
        }

        private void PopulateResults()
        {
            foreach (PrincipalIncident incident in _results.SelectMany(r => r.Incidents)
                         .OrderByDescending(i => i.SeverityScore).ThenBy(i => i.FileName).ThenBy(i => i.StartTimeMs))
                _incidentRows.Add(IncidentRow.FromIncident(incident));

            var orderedFindings = _results.SelectMany(r => r.Findings)
                .OrderByDescending(f => f.SeverityRank)
                .ThenBy(f => f.FileName)
                .ThenBy(f => f.TimeMs)
                .ToList();

            foreach (var finding in orderedFindings)
                _findingRows.Add(FindingRow.FromFinding(finding));

            foreach (var item in _results.SelectMany(r => r.Timeline)
                         .OrderBy(t => t.FileName).ThenBy(t => t.TimeMs))
                _timelineRows.Add(TimelineRow.FromEvent(item));

            foreach (var result in _results)
            {
                foreach (var pair in result.MessageCounts.OrderByDescending(p => p.Value).ThenBy(p => p.Key))
                {
                    string fields = result.MessageFields.ContainsKey(pair.Key)
                        ? string.Join(", ", result.MessageFields[pair.Key].ToArray())
                        : string.Empty;
                    _messageRows.Add(new MessageRow
                    {
                        File = Path.GetFileName(result.FilePath),
                        Message = pair.Key,
                        Count = pair.Value.ToString("N0", CultureInfo.InvariantCulture),
                        Fields = fields
                    });
                }
            }

            _summaryBox.Text = ReportBuilder.BuildPlainText(_results, _hebrew);
            _storyBox.Text = ReportBuilder.BuildNarrative(_results, _hebrew);
            ApplyReportTextDirection(_summaryBox);
            ApplyReportTextDirection(_storyBox);
            _exportButton.Enabled = _results.Count > 0;
            _openNativeButton.Enabled = _selectedFiles.Length > 0;
            _reanalyzeButton.Enabled = _selectedFiles.Length > 0;

            PopulateResultSelector();
            ColorSeverityRows();
            UpdateVisuals();
        }

        private void PopulateResultSelector()
        {
            _resultSelector.BeginUpdate();
            try
            {
                _resultSelector.Items.Clear();
                foreach (var result in _results)
                    _resultSelector.Items.Add(Path.GetFileName(result.FilePath));

                if (_resultSelector.Items.Count > 0)
                    _resultSelector.SelectedIndex = 0;
            }
            finally
            {
                _resultSelector.EndUpdate();
            }
        }

        private void ResultSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateVisuals();
        }

        private void GraphSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateGraph();
        }

        private void FindingsGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SyncVisualsFromCurrentSelection();
            OpenSelectedInNativeBrowser();
        }

        private void IncidentsGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SyncVisualsFromCurrentSelection();
            OpenSelectedInNativeBrowser();
        }

        private void TimelineGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SyncVisualsFromCurrentSelection();
            OpenSelectedInNativeBrowser();
        }

        private void SyncVisualsFromCurrentSelection()
        {
            string file = GetSelectedFile();
            double timeMs = GetSelectedTimeMs();
            if (!string.IsNullOrEmpty(file))
                SelectResultByFile(file);
            _mapPanel.SetTime(timeMs, true);
            _pilotCommandPanel.SetTime(timeMs);
            UpdateGraph(timeMs);
        }

        private double GetSelectedTimeMs()
        {
            if (IsSelectedTab("Incidents") && _incidentsGrid.CurrentRow != null)
            {
                var incidentRow = _incidentsGrid.CurrentRow.DataBoundItem as IncidentRow;
                return incidentRow == null ? 0 : incidentRow.TimeMs;
            }

            if (IsSelectedTab("Pilot & GCS") && _gcsCommandsGrid.CurrentRow != null)
            {
                var commandRow = _gcsCommandsGrid.CurrentRow.DataBoundItem as GcsCommandRow;
                return commandRow == null ? 0 : commandRow.TimeMs;
            }

            if (IsSelectedTab("Timeline") &&
                _timelineGrid.CurrentRow != null)
            {
                var row = _timelineGrid.CurrentRow.DataBoundItem as TimelineRow;
                return row == null ? 0 : row.TimeMs;
            }

            if (_findingsGrid.CurrentRow != null)
            {
                var row = _findingsGrid.CurrentRow.DataBoundItem as FindingRow;
                return row == null ? 0 : row.TimeMs;
            }

            return 0;
        }

        private void SelectResultByFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            string fileName = Path.GetFileName(filePath);
            for (int i = 0; i < _resultSelector.Items.Count; i++)
            {
                if (string.Equals(Convert.ToString(_resultSelector.Items[i], CultureInfo.InvariantCulture), fileName, StringComparison.OrdinalIgnoreCase))
                {
                    _resultSelector.SelectedIndex = i;
                    return;
                }
            }
        }

        private LogAnalysisResult GetVisualizedResult()
        {
            if (_results.Count == 0)
                return null;

            if (_resultSelector.SelectedIndex >= 0 && _resultSelector.SelectedIndex < _results.Count)
                return _results[_resultSelector.SelectedIndex];

            return _results[0];
        }

        private void UpdateVisuals()
        {
            var result = GetVisualizedResult();
            _mapPanel.Result = result;
            _pilotCommandPanel.Result = result;
            PopulateGcsCommands(result);
            if (result == null)
            {
                _graphControl.Result = null;
                _graphControl.Invalidate();
                _pilotCommandPanel.SetTime(0);
                return;
            }

            _visualTimeMs = result.FirstTimeMs;
            _pilotCommandPanel.SetTime(result.FirstTimeMs);
            UpdateGraph(result.FirstTimeMs);
            if (IsSelectedTab("Values"))
                UpdateValues(result.FirstTimeMs);
        }

        private void PopulateGcsCommands(LogAnalysisResult result)
        {
            _updatingGcsCommands = true;
            _gcsCommandRows.RaiseListChangedEvents = false;
            try
            {
                _gcsCommandRows.Clear();
                if (result == null)
                {
                    _gcsCommandStatusLabel.Text = Localization.Ui("No log selected.", _hebrew);
                    return;
                }
                foreach (MavCommandRecord command in result.MavCommands.OrderBy(c => c.TimeMs))
                    _gcsCommandRows.Add(GcsCommandRow.FromCommand(command));
                _gcsCommandStatusLabel.Text = result.MavCommands.Count == 0
                    ? (_hebrew
                        ? "לא נמצאו רשומות MAVC. אין בכך הוכחה שלא בוצעו פעולות GCS; ייתכן שהקושחה או פרופיל הרישום לא תיעדו אותן."
                        : "No MAVC records were found. This does not prove that no GCS actions occurred; the firmware/log may not have recorded them.")
                    : (_hebrew
                        ? result.MavCommands.Count.ToString("N0", CultureInfo.InvariantCulture) + " פקודות MAVLink תועדו. בחר שורה כדי לעבור לזמן האירוע; לחיצה כפולה תפתח את הרשומה הגולמית."
                        : result.MavCommands.Count.ToString("N0", CultureInfo.InvariantCulture) + " executed MAVLink command(s). Select a row to seek; double-click to open the raw record.");
            }
            finally
            {
                _gcsCommandRows.RaiseListChangedEvents = true;
                _gcsCommandRows.ResetBindings();
                _gcsCommandsGrid.ClearSelection();
                _updatingGcsCommands = false;
            }
        }

        private void UpdateGraph()
        {
            UpdateGraph(0);
        }

        private void UpdateGraph(double highlightTimeMs)
        {
            var result = GetVisualizedResult();
            _graphControl.Result = result;
            _graphControl.GraphName = CanonicalGraphName(_graphSelector.SelectedIndex);
            _graphControl.HighlightTimeMs = highlightTimeMs;
            _visualTimeMs = highlightTimeMs;
            _pilotCommandPanel.SetTime(highlightTimeMs);
            _graphControl.Invalidate();
            if (IsSelectedTab("Values"))
                UpdateValues(highlightTimeMs);
        }

        private void MapPanel_TimeChanged(object sender, MapTimeChangedEventArgs e)
        {
            UpdateGraph(e.TimeMs);
            _pilotCommandPanel.SetTime(e.TimeMs);
        }

        private void GraphControl_TimeSelected(object sender, MapTimeChangedEventArgs e)
        {
            _mapPanel.SetTime(e.TimeMs, true);
            _pilotCommandPanel.SetTime(e.TimeMs);
        }

        private void GcsCommandsGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (_updatingGcsCommands || _gcsCommandsGrid.CurrentRow == null)
                return;
            var row = _gcsCommandsGrid.CurrentRow.DataBoundItem as GcsCommandRow;
            if (row == null)
                return;
            _mapPanel.SetTime(row.TimeMs, true);
            _pilotCommandPanel.SetTime(row.TimeMs);
            UpdateGraph(row.TimeMs);
        }

        private void GcsCommandsGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            GcsCommandsGrid_SelectionChanged(sender, EventArgs.Empty);
            OpenSelectedInNativeBrowser();
        }

        private void GraphResetButton_Click(object sender, EventArgs e)
        {
            _graphControl.ResetView();
        }

        private void GraphZoomInButton_Click(object sender, EventArgs e)
        {
            _graphControl.ZoomBy(0.65);
        }

        private void GraphZoomOutButton_Click(object sender, EventArgs e)
        {
            _graphControl.ZoomBy(1.55);
        }

        private void GraphShowAllButton_Click(object sender, EventArgs e)
        {
            _graphControl.ShowAllSeries();
        }

        private void Tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsSelectedTab("Values"))
                UpdateValues(_visualTimeMs);
        }

        private void ValuesViewSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateValues(_visualTimeMs);
        }

        private void UpdateValues(double timeMs)
        {
            LogAnalysisResult result = GetVisualizedResult();
            _valueRows.RaiseListChangedEvents = false;
            try
            {
                _valueRows.Clear();
                if (result == null)
                {
                    _valuesTimeLabel.Text = "No log selected.";
                    return;
                }

                double t = timeMs > 0 ? timeMs : result.FirstTimeMs;
                t = Math.Max(result.FirstTimeMs, Math.Min(result.LastTimeMs, t));
                _visualTimeMs = t;
                string phase = FlightStateResolver.StateAt(result.FlightStates, t);
                _valuesTimeLabel.Text = _hebrew
                    ? "זמן " + FormatUiTime(result, t) + "   •   " + Localization.Phase(phase) + "   •   " + ModeAtUiTime(result, t)
                    : "Cursor " + FormatUiTime(result, t) + "   •   " + phase + "   •   " + ModeAtUiTime(result, t);

                if (_valuesViewSelector.SelectedIndex == 1)
                    PopulateNormalizedValueRows(result, t);
                else
                    PopulateSnapshotValueRows(result, t);
            }
            finally
            {
                _valueRows.RaiseListChangedEvents = true;
                _valueRows.ResetBindings();
            }
        }

        private void PopulateNormalizedValueRows(LogAnalysisResult result, double timeMs)
        {
            foreach (KeyValuePair<string, List<NormalizedSignalSample>> pair in result.NormalizedSignals.OrderBy(p => p.Key))
            {
                NormalizedSignalSample sample = FindNearestNormalized(pair.Value, timeMs);
                if (sample == null) continue;
                string category = pair.Key.Contains(".") ? pair.Key.Substring(0, pair.Key.IndexOf('.')) : "normalized";
                _valueRows.Add(ValueRow.From(category, pair.Key, sample.Value, sample.Unit, sample.Source, sample.TimeMs, timeMs, sample.Phase,
                    "Canonical engineering-unit signal. DataFlash parser scaling is trusted; no magnitude-based unit guessing is applied.", result));
            }
        }

        private void PopulateSnapshotValueRows(LogAnalysisResult result, double timeMs)
        {
            AddTextValue("Flight", "Resolved state", FlightStateResolver.StateAt(result.FlightStates, timeMs), "resolver", timeMs, timeMs, result, "One shared resolved flight-state timeline.");
            AddTextValue("Flight", "Mode", ModeAtUiTime(result, timeMs), "MODE", timeMs, timeMs, result, "Most recent logged mode at or before the cursor.");

            foreach (KeyValuePair<string, List<GpsSample>> pair in result.Gps.OrderBy(p => SensorAssessmentBuilder.InferGpsOrdinal(p.Key)).ThenBy(p => p.Key))
            {
                GpsSample gps = FindNearestTimed(pair.Value, timeMs, x => x.TimeMs);
                if (gps == null) continue;
                int ordinal = SensorAssessmentBuilder.InferGpsOrdinal(pair.Key);
                string prefix = "GPS" + ordinal.ToString(CultureInfo.InvariantCulture);
                string source = pair.Key + " / " + GpsDisplayHelper.GetGpsTypeName(result, ordinal);
                AddNumberValue("GPS", prefix + " fix status", gps.Status, "", source, gps.TimeMs, timeMs, gps.Phase, "ArduPilot/MAVLink fix-type numeric value.", result);
                AddNumberValue("GPS", prefix + " satellites", gps.Sats, "count", source, gps.TimeMs, timeMs, gps.Phase, "Satellites visible/used as logged by this receiver.", result);
                AddNumberValue("GPS", prefix + " HDOP", gps.Hdop, "", source, gps.TimeMs, timeMs, gps.Phase, "Horizontal dilution of precision in parsed engineering units.", result);
                AddNumberValue("GPS", prefix + " speed", gps.Speed, "m/s", source, gps.TimeMs, timeMs, gps.Phase, "Ground speed from this GPS receiver.", result);
                AddNumberValue("GPS", prefix + " altitude", gps.Altitude, "m", source, gps.TimeMs, timeMs, gps.Phase, "GPS altitude; datum may differ from CTUN/home-relative altitude.", result);
                AddNumberValue("GPS", prefix + " latitude", gps.Lat, "deg", source, gps.TimeMs, timeMs, gps.Phase, "Raw parsed geographic coordinate.", result, "0.0000000");
                AddNumberValue("GPS", prefix + " longitude", gps.Lng, "deg", source, gps.TimeMs, timeMs, gps.Phase, "Raw parsed geographic coordinate.", result, "0.0000000");
                AddNumberValue("GPS", prefix + " in use", gps.Used, "bool", source, gps.TimeMs, timeMs, gps.Phase, "When present, U=1 indicates this GPS is in use.", result);
            }

            RcInputSample rc = FindNearestTimed(result.RcInputs, timeMs, x => x.TimeMs);
            if (rc != null)
            {
                AddNumberValue("Pilot", "Roll input", rc.Roll, "%", "RCIN", rc.TimeMs, timeMs, rc.Phase, "Normalized using RCMAP and RC calibration parameters when available.", result);
                AddNumberValue("Pilot", "Pitch input", rc.Pitch, "%", "RCIN", rc.TimeMs, timeMs, rc.Phase, "Normalized using RCMAP and RC calibration parameters when available.", result);
                AddNumberValue("Pilot", "Throttle input", rc.Throttle, "%", "RCIN", rc.TimeMs, timeMs, rc.Phase, "0–100% normalized throttle input.", result);
                AddNumberValue("Pilot", "Yaw input", rc.Yaw, "%", "RCIN", rc.TimeMs, timeMs, rc.Phase, "Normalized using RCMAP and RC calibration parameters when available.", result);
            }

            AttitudeSample att = FindNearestTimed(result.Attitudes, timeMs, x => x.TimeMs);
            if (att != null)
            {
                AddNumberValue("Attitude", "Roll actual", att.Roll, "deg", "ATT", att.TimeMs, timeMs, att.Phase, "Canonical vehicle attitude.", result);
                AddNumberValue("Attitude", "Roll desired", att.DesiredRoll, "deg", "ATT", att.TimeMs, timeMs, att.Phase, "Controller-requested attitude when logged.", result);
                AddNumberValue("Attitude", "Roll error", att.RollError, "deg", "ATT", att.TimeMs, timeMs, att.Phase, "Absolute desired-versus-actual error.", result);
                AddNumberValue("Attitude", "Pitch actual", att.Pitch, "deg", "ATT", att.TimeMs, timeMs, att.Phase, "Canonical vehicle attitude.", result);
                AddNumberValue("Attitude", "Pitch desired", att.DesiredPitch, "deg", "ATT", att.TimeMs, timeMs, att.Phase, "Controller-requested attitude when logged.", result);
                AddNumberValue("Attitude", "Pitch error", att.PitchError, "deg", "ATT", att.TimeMs, timeMs, att.Phase, "Absolute desired-versus-actual error.", result);
                AddNumberValue("Attitude", "Yaw actual", att.Yaw, "deg", "ATT", att.TimeMs, timeMs, att.Phase, "Canonical vehicle yaw.", result);
            }

            RateSample rate = FindNearestTimed(result.Rates, timeMs, x => x.TimeMs);
            if (rate != null)
            {
                AddNumberValue("Rates", "Roll desired", rate.RollDesired, "deg/s", "RATE", rate.TimeMs, timeMs, rate.Phase, "Desired angular rate.", result);
                AddNumberValue("Rates", "Roll actual", rate.RollActual, "deg/s", "RATE", rate.TimeMs, timeMs, rate.Phase, "Achieved angular rate.", result);
                AddNumberValue("Rates", "Pitch desired", rate.PitchDesired, "deg/s", "RATE", rate.TimeMs, timeMs, rate.Phase, "Desired angular rate.", result);
                AddNumberValue("Rates", "Pitch actual", rate.PitchActual, "deg/s", "RATE", rate.TimeMs, timeMs, rate.Phase, "Achieved angular rate.", result);
                AddNumberValue("Rates", "Yaw desired", rate.YawDesired, "deg/s", "RATE", rate.TimeMs, timeMs, rate.Phase, "Desired angular rate.", result);
                AddNumberValue("Rates", "Yaw actual", rate.YawActual, "deg/s", "RATE", rate.TimeMs, timeMs, rate.Phase, "Achieved angular rate.", result);
            }

            AltitudeSample alt = FindNearestTimed(result.Altitudes, timeMs, x => x.TimeMs);
            if (alt != null)
            {
                AddNumberValue("Vertical", "Altitude", alt.Altitude, "m", "CTUN", alt.TimeMs, timeMs, alt.Phase, "CTUN/vehicle altitude in logged reference frame.", result);
                AddNumberValue("Vertical", "Desired altitude", alt.DesiredAltitude, "m", "CTUN", alt.TimeMs, timeMs, alt.Phase, "Controller altitude target when logged.", result);
                AddNumberValue("Vertical", "Climb rate", alt.ClimbRate, "m/s", "CTUN", alt.TimeMs, timeMs, alt.Phase, "Estimated vertical rate.", result);
                AddNumberValue("Vertical", "Desired climb rate", alt.DesiredClimbRate, "m/s", "CTUN", alt.TimeMs, timeMs, alt.Phase, "Controller vertical-rate target.", result);
            }

            foreach (KeyValuePair<string, List<BatterySample>> pair in result.Batteries.OrderBy(p => p.Key))
            {
                BatterySample bat = FindNearestTimed(pair.Value, timeMs, x => x.TimeMs);
                if (bat == null) continue;
                AddNumberValue("Battery", pair.Key + " voltage", bat.Voltage, "V", pair.Key, bat.TimeMs, timeMs, bat.Phase, "Measured battery voltage when monitoring is enabled.", result);
                AddNumberValue("Battery", pair.Key + " current", bat.Current, "A", pair.Key, bat.TimeMs, timeMs, bat.Phase, "Measured battery current when available.", result);
            }

            VibeSample vibe = FindNearestTimed(result.Vibes, timeMs, x => x.TimeMs);
            if (vibe != null)
            {
                string src = "VIBE IMU " + (vibe.Instance + 1).ToString(CultureInfo.InvariantCulture);
                AddNumberValue("Vibration", "Vibe X", vibe.X, "m/s²", src, vibe.TimeMs, timeMs, vibe.Phase, "Processed acceleration vibration.", result);
                AddNumberValue("Vibration", "Vibe Y", vibe.Y, "m/s²", src, vibe.TimeMs, timeMs, vibe.Phase, "Processed acceleration vibration.", result);
                AddNumberValue("Vibration", "Vibe Z", vibe.Z, "m/s²", src, vibe.TimeMs, timeMs, vibe.Phase, "Processed acceleration vibration.", result);
                AddNumberValue("Vibration", "Clipping counter", vibe.Clip, "count", src, vibe.TimeMs, timeMs, vibe.Phase, "Cumulative clipping counter for this IMU when available.", result);
            }

            MotorSample motor = FindNearestTimed(result.Motors, timeMs, x => x.TimeMs);
            if (motor != null && motor.Values != null)
                foreach (KeyValuePair<int, double> ch in motor.Values.OrderBy(p => p.Key).Take(16))
                    AddNumberValue("Outputs", "RCOU channel " + ch.Key.ToString(CultureInfo.InvariantCulture), ch.Value, "raw", "RCOU", motor.TimeMs, timeMs, motor.Phase, "Commanded output only; this is not measured motor RPM or thrust.", result);

            foreach (EkfSample ekf in result.Ekf.GroupBy(x => x.Core).Select(g => FindNearestTimed(g.OrderBy(x => x.TimeMs).ToList(), timeMs, x => x.TimeMs)).Where(x => x != null).OrderBy(x => x.Core))
            {
                string src = ekf.Source + (ekf.Core >= 0 ? " core " + ekf.Core.ToString(CultureInfo.InvariantCulture) : string.Empty);
                AddNumberValue("Estimator", "Velocity variance", ekf.VelocityVariance, "", src, ekf.TimeMs, timeMs, ekf.Phase, "Estimator diagnostic; interpretation depends on message/firmware definition.", result);
                AddNumberValue("Estimator", "Position variance", ekf.PositionVariance, "", src, ekf.TimeMs, timeMs, ekf.Phase, "Estimator diagnostic; interpretation depends on message/firmware definition.", result);
                AddNumberValue("Estimator", "Height variance", ekf.HeightVariance, "", src, ekf.TimeMs, timeMs, ekf.Phase, "Estimator diagnostic; interpretation depends on message/firmware definition.", result);
            }
        }

        private void AddTextValue(string category, string signal, string value, string source, double sampleTimeMs, double cursorTimeMs, LogAnalysisResult result, string notes)
        {
            _valueRows.Add(ValueRow.FromText(category, signal, value, source, sampleTimeMs, cursorTimeMs, FlightStateResolver.StateAt(result.FlightStates, sampleTimeMs), notes, result));
        }

        private void AddNumberValue(string category, string signal, double? value, string unit, string source, double sampleTimeMs, double cursorTimeMs, string phase, string notes, LogAnalysisResult result, string format = "0.###")
        {
            if (!value.HasValue || double.IsNaN(value.Value) || double.IsInfinity(value.Value)) return;
            _valueRows.Add(ValueRow.From(category, signal, value.Value, unit, source, sampleTimeMs, cursorTimeMs, phase, notes, result, format));
        }

        private static T FindNearestTimed<T>(IList<T> items, double timeMs, Func<T, double> getTime) where T : class
        {
            if (items == null || items.Count == 0) return null;
            int low = 0, high = items.Count - 1;
            while (low < high)
            {
                int mid = (low + high) / 2;
                if (getTime(items[mid]) < timeMs) low = mid + 1; else high = mid;
            }
            int a = low;
            int b = Math.Max(0, low - 1);
            return Math.Abs(getTime(items[a]) - timeMs) < Math.Abs(getTime(items[b]) - timeMs) ? items[a] : items[b];
        }

        private static NormalizedSignalSample FindNearestNormalized(IList<NormalizedSignalSample> items, double timeMs)
        {
            return FindNearestTimed(items, timeMs, x => x.TimeMs);
        }

        private static string FormatUiTime(LogAnalysisResult result, double timeMs)
        {
            double relative = result == null ? timeMs : Math.Max(0, timeMs - result.FirstTimeMs);
            TimeSpan t = TimeSpan.FromMilliseconds(relative);
            return string.Format(CultureInfo.InvariantCulture, "{0:00}:{1:00}:{2:00}.{3:000}", (int)t.TotalHours, t.Minutes, t.Seconds, t.Milliseconds);
        }

        private static string ModeAtUiTime(LogAnalysisResult result, double timeMs)
        {
            TimelineEvent mode = result == null ? null : result.Timeline.Where(x => x.Type == "Mode" && x.TimeMs <= timeMs).OrderByDescending(x => x.TimeMs).FirstOrDefault();
            return mode == null ? "Unknown" : mode.Text.Replace("Flight mode changed to ", string.Empty);
        }

        private void ColorSeverityRows()
        {
            ColorSeverityGrid(_findingsGrid);
            ColorSeverityGrid(_incidentsGrid);
        }

        private static void ColorSeverityGrid(DataGridView grid)
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.Cells.Count == 0) continue;
                string severity = Convert.ToString(row.Cells[0].Value, CultureInfo.InvariantCulture);
                DataGridViewCell cell = row.Cells[0];
                cell.Style.Font = new Font("Segoe UI Semibold", 8.8f, FontStyle.Bold);
                cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                if (severity == "Critical") { cell.Style.BackColor = UiPalette.CriticalSoft; cell.Style.ForeColor = UiPalette.Critical; }
                else if (severity == "Warning") { cell.Style.BackColor = UiPalette.WarningSoft; cell.Style.ForeColor = UiPalette.Warning; }
                else if (severity == "Advisory") { cell.Style.BackColor = UiPalette.AdvisorySoft; cell.Style.ForeColor = UiPalette.Advisory; }
                else { cell.Style.BackColor = UiPalette.SuccessSoft; cell.Style.ForeColor = UiPalette.Success; }
            }
        }

        private void GridSelectionChanged(object sender, EventArgs e)
        {
            _openNativeButton.Enabled = GetSelectedFile() != null;
        }

        private string GetSelectedFile()
        {
            if (IsSelectedTab("Incidents") && _incidentsGrid.CurrentRow != null)
            {
                var incidentRow = _incidentsGrid.CurrentRow.DataBoundItem as IncidentRow;
                return incidentRow == null ? null : incidentRow.FilePath;
            }

            if (IsSelectedTab("Pilot & GCS") && _gcsCommandsGrid.CurrentRow != null)
            {
                var commandRow = _gcsCommandsGrid.CurrentRow.DataBoundItem as GcsCommandRow;
                return commandRow == null ? null : commandRow.FilePath;
            }

            if (IsSelectedTab("Timeline") &&
                _timelineGrid.CurrentRow != null)
            {
                var row = _timelineGrid.CurrentRow.DataBoundItem as TimelineRow;
                return row == null ? null : row.FilePath;
            }

            if (_findingsGrid.CurrentRow != null)
            {
                var row = _findingsGrid.CurrentRow.DataBoundItem as FindingRow;
                return row == null ? null : row.FilePath;
            }

            return _selectedFiles.FirstOrDefault();
        }

        private int GetSelectedLine()
        {
            if (IsSelectedTab("Incidents") && _incidentsGrid.CurrentRow != null)
            {
                var incidentRow = _incidentsGrid.CurrentRow.DataBoundItem as IncidentRow;
                return incidentRow == null ? 0 : incidentRow.LineNumber;
            }

            if (IsSelectedTab("Pilot & GCS") && _gcsCommandsGrid.CurrentRow != null)
            {
                var commandRow = _gcsCommandsGrid.CurrentRow.DataBoundItem as GcsCommandRow;
                return commandRow == null ? 0 : commandRow.LineNumber;
            }

            if (IsSelectedTab("Timeline") &&
                _timelineGrid.CurrentRow != null)
            {
                var row = _timelineGrid.CurrentRow.DataBoundItem as TimelineRow;
                return row == null ? 0 : row.LineNumber;
            }

            if (_findingsGrid.CurrentRow != null)
            {
                var row = _findingsGrid.CurrentRow.DataBoundItem as FindingRow;
                return row == null ? 0 : row.LineNumber;
            }

            return 0;
        }

        private void OpenNativeButton_Click(object sender, EventArgs e)
        {
            OpenSelectedInNativeBrowser();
        }

        private void OpenSelectedInNativeBrowser()
        {
            string file = GetSelectedFile();
            if (string.IsNullOrEmpty(file) || !File.Exists(file))
                return;

            try
            {
                var browser = new LogBrowse();
                browser.logfilename = file;
                browser.Show(MainV2.instance);

                int line = GetSelectedLine();
                if (line > 0)
                {
                    Clipboard.SetText(line.ToString(CultureInfo.InvariantCulture));
                    _statusLabel.Text = _hebrew
                        ? "Log Browse נפתח. שורת הראיה " + line.ToString(CultureInfo.InvariantCulture) + " הועתקה; לחץ Ctrl+G ב-Log Browse והדבק אותה."
                        : "Opened Log Browse. Evidence line " + line.ToString(CultureInfo.InvariantCulture) +
                          " was copied; press Ctrl+G in Log Browse and paste it.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Could not open Log Browse",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            if (_results.Count == 0)
                return;

            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "HTML report|*.html";
                dialog.FileName = (_hebrew ? "Drone_Log_Investigation_HE_" : "Drone_Log_Investigation_") + DateTime.Now.ToString("yyyyMMdd_HHmm", CultureInfo.InvariantCulture) + ".html";
                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;

                File.WriteAllText(dialog.FileName, ReportBuilder.BuildHtml(_results, _hebrew), Encoding.UTF8);
                _statusLabel.Text = _hebrew ? "הדוח יוצא אל " + dialog.FileName : "Report exported to " + dialog.FileName;
                try
                {
                    Process.Start(dialog.FileName);
                }
                catch
                {
                    // Export succeeded even when Windows has no HTML file association.
                }
            }
        }
    }

    internal sealed class LogAnalyzer
    {
        private readonly string _filePath;
        private readonly LogAnalysisResult _result;
        private readonly Dictionary<string, double> _parameters = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> _lastMessages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<BatterySample>> _batteries = new Dictionary<string, List<BatterySample>>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<GpsSample>> _gps = new Dictionary<string, List<GpsSample>>(StringComparer.OrdinalIgnoreCase);
        private readonly List<VibeSample> _vibes = new List<VibeSample>();
        private readonly List<AttitudeSample> _attitudes = new List<AttitudeSample>();
        private readonly List<AltitudeSample> _altitudes = new List<AltitudeSample>();
        private readonly List<MotorSample> _motors = new List<MotorSample>();
        private readonly List<RcInputSample> _rcInputs = new List<RcInputSample>();
        private readonly List<MavCommandRecord> _mavCommands = new List<MavCommandRecord>();
        private readonly List<EkfSample> _ekf = new List<EkfSample>();
        private readonly List<AhrsSample> _ahrs2 = new List<AhrsSample>();
        private readonly List<ImuSample> _imu = new List<ImuSample>();
        private readonly List<RateSample> _rates = new List<RateSample>();
        private readonly List<MissionCommandRecord> _missionCommands = new List<MissionCommandRecord>();
        private readonly List<ReferencePointRecord> _referencePoints = new List<ReferencePointRecord>();
        private readonly List<StateEvidence> _stateEvidence = new List<StateEvidence>();
        private readonly FirmwareProfile _firmware = new FirmwareProfile();
        private readonly List<double> _attitudeTimes = new List<double>();
        private readonly Dictionary<string, CounterPeak> _counterPeaks = new Dictionary<string, CounterPeak>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, SampleCapAssessment> _sampleCaps = new Dictionary<string, SampleCapAssessment>(StringComparer.OrdinalIgnoreCase);
        private readonly List<string> _itemParseErrorSamples = new List<string>();
        private string _currentMode = "Unknown";
        private bool _armed;
        private double _currentAltitude;
        private double _currentClimbRate;
        private int _currentRcOverrideMask;
        private int _currentRcFlags;
        private double _firstTime = double.MaxValue;
        private double _lastTime;
        private int _sampleLimit = 250000;

        private LogAnalyzer(string filePath)
        {
            _filePath = filePath;
            _result = new LogAnalysisResult { FilePath = filePath };
        }

        public static LogAnalysisResult Analyze(string filePath)
        {
            var analyzer = new LogAnalyzer(filePath);
            return analyzer.Run();
        }

        private LogAnalysisResult Run()
        {
            try
            {
                using (var buffer = new DFLogBuffer(_filePath))
                {
                    DFLog df = buffer.dflog;
                    _result.TotalLines = buffer.Count;

                    foreach (var format in df.logformat)
                    {
                        _result.MessageFields[format.Key] = format.Value.FieldNames.ToList();
                        _firmware.MessageFormats.Add(format.Key);
                    }

                    string[] relevant = SelectRelevantTypes(df.logformat.Keys).ToArray();
                    foreach (DFLog.DFItem item in buffer.GetEnumeratorType(relevant))
                    {
                        try
                        {
                            ProcessItem(item, df);
                        }
                        catch (Exception itemException)
                        {
                            _result.ItemParseErrors++;
                            if (_itemParseErrorSamples.Count < 3)
                                _itemParseErrorSamples.Add("line " + item.lineno.ToString(CultureInfo.InvariantCulture) + " type " + Safe(item.msgtype) + ": " + itemException.Message);
                        }
                    }

                    try
                    {
                        _result.Integrity = LogIntegrityBuilder.InspectPhysicalFile(_filePath, buffer.FMT.ToDictionary(p => p.Key, p => p.Value.Item1), buffer.Count);
                    }
                    catch (Exception integrityException)
                    {
                        _result.Integrity = new LogIntegrityAssessment
                        {
                            Status = "Uncertain",
                            Reason = "Physical file-ending inspection could not complete: " + integrityException.Message,
                            Confidence = "Low"
                        };
                    }
                }

                if (_firstTime == double.MaxValue)
                    _firstTime = 0;

                _result.FirstTimeMs = _firstTime;
                _result.LastTimeMs = _lastTime;
                _result.DurationSeconds = Math.Max(0, (_lastTime - _firstTime) / 1000.0);
                _result.Parameters = new Dictionary<string, double>(_parameters, StringComparer.OrdinalIgnoreCase);

                // Copy parsed data before interpretation so every later subsystem works from one shared data model.
                _result.Batteries = _batteries.ToDictionary(p => p.Key, p => p.Value, StringComparer.OrdinalIgnoreCase);
                _result.Gps = _gps.ToDictionary(p => p.Key, p => p.Value, StringComparer.OrdinalIgnoreCase);
                _result.Vibes = new List<VibeSample>(_vibes);
                _result.Attitudes = new List<AttitudeSample>(_attitudes);
                _result.Altitudes = new List<AltitudeSample>(_altitudes);
                _result.Motors = new List<MotorSample>(_motors);
                _result.RcInputs = new List<RcInputSample>(_rcInputs);
                _result.MavCommands = new List<MavCommandRecord>(_mavCommands);
                _result.Ekf = new List<EkfSample>(_ekf);
                _result.Ahrs2 = new List<AhrsSample>(_ahrs2);
                _result.Imu = new List<ImuSample>(_imu);
                _result.Rates = new List<RateSample>(_rates);
                _result.MissionCommands = new List<MissionCommandRecord>(_missionCommands);
                _result.ReferencePoints = new List<ReferencePointRecord>(_referencePoints);
                _result.Firmware = _firmware;
                _result.SampleCaps = _sampleCaps.Values.Where(s => s.DroppedSamples > 0).OrderByDescending(s => s.DroppedSamples).ToList();

                _result.FirmwareMessages = _result.Timeline
                    .Where(t => t.Type == "MSG")
                    .Select(t => t.Text)
                    .Take(30)
                    .ToList();

                // RC normalization depends on parameters and is useful to later correlation stages.
                FinalizeRcInputs();

                // One state resolver is authoritative for every report section and every sample phase.
                _result.FlightStates = FlightStateResolver.Resolve(_result, _stateEvidence);
                LogIntegrityBuilder.FinalizeWithState(_result);
                RephaseAllResolvedData();

                // Configuration/firmware-aware interpretation is built before threshold findings.
                _result.SensorAssessments = SensorAssessmentBuilder.Build(_result);
                _result.DataAvailability = DataAvailabilityBuilder.Build(_result);

                FinalizeGeneralChecks();
                FinalizeBatteryChecks();
                FinalizeVibrationChecks();
                FinalizeGpsChecks();
                FinalizeAttitudeChecks();
                FinalizeAltitudeChecks();
                FinalizeMotorChecks();
                FinalizeEkfChecks();
                FinalizePerformanceChecks();

                _result.NormalizedSignals = NormalizedDataBuilder.Build(_result);

                // Correlation stage: independent findings become principal incidents only when
                // multiple signals agree in time. Event and root-cause confidence are separate.
                // Keep this stage isolated so an unexpected correlation edge case cannot invalidate
                // the already-parsed flight or prevent the rest of the investigator from working.
                try
                {
                    IncidentCorrelationEngine.Run(_result);
                }
                catch (Exception correlationException)
                {
                    AddFinding("Advisory", "High", "Investigation engine",
                        "Correlation stage could not complete",
                        correlationException.Message,
                        "The underlying log remains available. Review the detailed findings and native Log Browse; this correlation-stage failure is not evidence of a vehicle fault.",
                        _result.LastTimeMs, 0, FlightStateResolver.StateAt(_result.FlightStates, _result.LastTimeMs));
                }

                LogIntegrityBuilder.FinalizeWithState(_result);

                _result.Findings.Sort(delegate(Finding a, Finding b)
                {
                    int severity = b.SeverityRank.CompareTo(a.SeverityRank);
                    return severity != 0 ? severity : a.TimeMs.CompareTo(b.TimeMs);
                });
            }
            catch (Exception ex)
            {
                _result.ParseError = ex.ToString();
                AddFinding("Critical", "High", "Log parser", "The log could not be fully parsed",
                    ex.Message, "Confirm the file is a complete ArduPilot DataFlash .BIN/.LOG and try opening it in Mission Planner Log Browse.",
                    0, 0, "Unknown");
            }

            return _result;
        }

        private static IEnumerable<string> SelectRelevantTypes(IEnumerable<string> keys)
        {
            var exact = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "MSG", "PARM", "MODE", "ERR", "EV", "ARM", "BAT", "BAT2", "POWR", "VIBE",
                "ATT", "AHR2", "RATE", "CTUN", "RCOU", "RCIN", "RCI2", "MAVC", "MAV", "PM", "GPS", "GPS2", "GPA", "GPA2", "BARO",
                "BAR2", "RFND", "AHR2", "ORGN", "CMD", "MISE", "XKFS", "NKFS", "DSF", "IMU", "IMU2", "IMU3"
            };

            foreach (string key in keys)
            {
                if (exact.Contains(key) ||
                    key.StartsWith("BAT", StringComparison.OrdinalIgnoreCase) ||
                    key.StartsWith("GPS", StringComparison.OrdinalIgnoreCase) ||
                    key.StartsWith("GPA", StringComparison.OrdinalIgnoreCase) ||
                    key.StartsWith("MAG", StringComparison.OrdinalIgnoreCase) ||
                    key.StartsWith("BAR", StringComparison.OrdinalIgnoreCase) ||
                    key.StartsWith("IMU", StringComparison.OrdinalIgnoreCase) ||
                    key.StartsWith("ESC", StringComparison.OrdinalIgnoreCase) ||
                    key.StartsWith("RPM", StringComparison.OrdinalIgnoreCase) ||
                    key.StartsWith("XKF", StringComparison.OrdinalIgnoreCase) ||
                    key.StartsWith("NKF", StringComparison.OrdinalIgnoreCase) ||
                    key.StartsWith("VIBE", StringComparison.OrdinalIgnoreCase))
                    yield return key;
            }
        }

        private void ProcessItem(DFLog.DFItem item, DFLog df)
        {
            if (string.IsNullOrEmpty(item.msgtype))
                return;

            double time = item.timems;
            if (!double.IsNaN(time) && !double.IsInfinity(time) && time >= 0)
            {
                _firstTime = Math.Min(_firstTime, time);
                _lastTime = Math.Max(_lastTime, time);
            }

            IncrementCount(item.msgtype);
            string type = item.msgtype.ToUpperInvariant();

            if (type == "PARM") ProcessParameter(item, df);
            else if (type == "MODE") ProcessMode(item, df);
            else if (type == "ERR") ProcessError(item, df);
            else if (type == "EV") ProcessEvent(item, df);
            else if (type == "MSG") ProcessMessage(item, df);
            else if (type == "ARM") ProcessArm(item, df);
            else if (type.StartsWith("BAT", StringComparison.Ordinal)) ProcessBattery(item, df);
            else if (type == "POWR") ProcessPower(item, df);
            else if (type.StartsWith("VIBE", StringComparison.Ordinal)) ProcessVibe(item, df);
            else if (type.StartsWith("GPS", StringComparison.Ordinal)) ProcessGps(item, df);
            else if (type == "ATT") ProcessAttitude(item, df);
            else if (type == "AHR2") ProcessAhrs2(item, df);
            else if (type == "RATE") ProcessRate(item, df);
            else if (type.StartsWith("IMU", StringComparison.Ordinal)) ProcessImu(item, df);
            else if (type == "CTUN") ProcessAltitude(item, df);
            else if (type == "RCOU") ProcessMotor(item, df);
            else if (type == "RCIN") ProcessRcInput(item, df);
            else if (type == "RCI2") ProcessRcInput2(item, df);
            else if (type == "MAVC") ProcessMavCommand(item, df);
            else if (type == "CMD" || type == "MISE") ProcessMissionCommand(item, df);
            else if (type == "ORGN") ProcessReferencePoint(item, df);
            else if (type == "PM") ProcessPerformance(item, df);
            else if (type.StartsWith("XKF", StringComparison.Ordinal) || type.StartsWith("NKF", StringComparison.Ordinal))
                ProcessEkf(item, df);
        }

        private void IncrementCount(string type)
        {
            if (!_result.MessageCounts.ContainsKey(type))
                _result.MessageCounts[type] = 0;
            _result.MessageCounts[type]++;
        }

        private void ProcessParameter(DFLog.DFItem item, DFLog df)
        {
            string name = GetString(item, df, "Name", "N");
            double? value = GetDouble(item, df, "Value", "V");
            if (!string.IsNullOrEmpty(name) && value.HasValue)
                _parameters[name.Trim()] = value.Value;
        }

        private void ProcessMode(DFLog.DFItem item, DFLog df)
        {
            string mode = GetString(item, df, "Mode", "M");
            if (string.IsNullOrEmpty(mode))
            {
                double? modeNum = GetDouble(item, df, "ModeNum", "Mode", "M");
                mode = modeNum.HasValue ? modeNum.Value.ToString("0", CultureInfo.InvariantCulture) : "Unknown";
            }

            if (!string.Equals(mode, _currentMode, StringComparison.OrdinalIgnoreCase))
            {
                _currentMode = mode;
                AddTimeline(item, "Mode", "Flight mode changed to " + mode);
            }
        }

        private void ProcessArm(DFLog.DFItem item, DFLog df)
        {
            double? state = GetDouble(item, df, "ArmState", "State", "Arm");
            if (state.HasValue)
            {
                _armed = state.Value > 0.5;
                AddTimeline(item, "Arming", _armed ? "Vehicle armed" : "Vehicle disarmed");
                _stateEvidence.Add(new StateEvidence
                {
                    TimeMs = item.timems,
                    Kind = _armed ? "ARM" : "DISARM",
                    Value = state.Value,
                    Text = "ARM message",
                    Line = item.lineno
                });
            }
        }

        private void ProcessError(DFLog.DFItem item, DFLog df)
        {
            string subsystem = GetString(item, df, "Subsys", "Subsystem");
            string code = GetString(item, df, "ECode", "Code");
            string evidence = "ERR message: subsystem=" + Safe(subsystem) + ", code=" + Safe(code) + ".";
            AddFinding("Warning", "High", "Autopilot event", "Autopilot recorded an ERR message",
                evidence, "Inspect the surrounding messages and graphs in Mission Planner Log Browse; identify the named subsystem before changing hardware or parameters.",
                item.timems, item.lineno, GetPhase());
            AddTimeline(item, "ERR", evidence);
        }

        private void ProcessEvent(DFLog.DFItem item, DFLog df)
        {
            int id = (int)(GetDouble(item, df, "Id", "Event", "EId") ?? -1);
            string eventName = id.ToString(CultureInfo.InvariantCulture);
            try
            {
                if (Enum.IsDefined(typeof(DFLog.Log_Event), (byte)id))
                    eventName = ((DFLog.Log_Event)(byte)id).ToString();
            }
            catch
            {
                // Numeric event remains useful when firmware event tables differ.
            }

            AddTimeline(item, "EV", "Event " + eventName + " (" + id.ToString(CultureInfo.InvariantCulture) + ")");
            _stateEvidence.Add(new StateEvidence
            {
                TimeMs = item.timems,
                Kind = "EVENT",
                Value = id,
                Text = eventName,
                Line = item.lineno
            });

            string upper = eventName.ToUpperInvariant();
            if (upper.Contains("CRASH") || upper.Contains("FAILSAFE") || upper.Contains("EKF") || upper.Contains("PARACHUTE"))
            {
                AddFinding(upper.Contains("CRASH") ? "Critical" : "Warning", "High", "Autopilot event",
                    "Significant autopilot event: " + eventName,
                    "EV id " + id.ToString(CultureInfo.InvariantCulture) + " was recorded.",
                    "Correlate the event with mode, attitude, battery, GPS and EKF data around this timestamp.",
                    item.timems, item.lineno, GetPhase());
            }
        }

        private void ProcessMessage(DFLog.DFItem item, DFLog df)
        {
            string text = GetString(item, df, "Message", "Msg", "Text");
            if (string.IsNullOrWhiteSpace(text))
                return;

            AddTimeline(item, "MSG", text);
            string normalized = text.Trim();
            FirmwareProfileParser.UpdateFromText(_firmware, normalized);
            string upper = normalized.ToUpperInvariant();

            if (_lastMessages.ContainsKey(upper) && _lastMessages[upper] == GetTimeText(item.timems))
                return;
            _lastMessages[upper] = GetTimeText(item.timems);

            string severity = null;
            if (ContainsAny(upper, "CRASH", "BROWNOUT", "INTERNAL ERROR", "WATCHDOG", "MOTOR FAILURE"))
                severity = "Critical";
            else if (ContainsAny(upper, "FAILSAFE", "UNHEALTHY", "ERROR", "LOST", "RESET", "GLITCH", "VIBRATION", "CLIP", "FENCE BREACH"))
                severity = "Warning";

            if (severity != null)
            {
                AddFinding(severity, "High", "Autopilot message", normalized,
                    "The firmware emitted this MSG record.",
                    "Review all data within at least 10 seconds before and after the message; do not treat the text alone as a root-cause conclusion.",
                    item.timems, item.lineno, GetPhase());
            }
        }

        private void ProcessBattery(DFLog.DFItem item, DFLog df)
        {
            double? voltage = GetDouble(item, df, "Volt", "Voltage", "V");
            double? current = GetDouble(item, df, "Curr", "Current", "I");
            double? used = GetDouble(item, df, "CurrTot", "Consumed", "Mah", "mAh");
            if (!voltage.HasValue && !current.HasValue)
                return;

            string key = item.msgtype;
            string instance = SafeInstance(item);
            if (!string.IsNullOrEmpty(instance))
                key += "[" + instance + "]";

            if (!_batteries.ContainsKey(key))
                _batteries[key] = new List<BatterySample>();
            AddCapped(_batteries[key], new BatterySample
            {
                TimeMs = item.timems,
                Voltage = voltage,
                Current = current,
                UsedMah = used,
                Line = item.lineno,
                Phase = GetPhase()
            }, "Battery:" + key);
        }

        private void ProcessPower(DFLog.DFItem item, DFLog df)
        {
            double? vcc = GetDouble(item, df, "Vcc", "VCC");
            if (vcc.HasValue)
            {
                double value = NormalizeVoltage(vcc.Value);
                TrackCounter("POWR.VccMin", value, item, false);
            }

            TrackCounterField(item, df, "POWR.Flags", false, "Flags");
        }

        private void ProcessVibe(DFLog.DFItem item, DFLog df)
        {
            double? x = GetDouble(item, df, "VibeX", "X");
            double? y = GetDouble(item, df, "VibeY", "Y");
            double? z = GetDouble(item, df, "VibeZ", "Z");
            double? modernClip = GetDouble(item, df, "Clip", "Clipping");
            int instance = (int)(GetDouble(item, df, "IMU", "I", "Instance") ?? 0);
            double? c0 = GetDouble(item, df, "Clip0", "Clp0");
            double? c1 = GetDouble(item, df, "Clip1", "Clp1");
            double? c2 = GetDouble(item, df, "Clip2", "Clp2");
            if (!x.HasValue && !y.HasValue && !z.HasValue && !modernClip.HasValue) return;

            AddCapped(_vibes, new VibeSample
            {
                TimeMs = item.timems,
                Instance = instance,
                X = x ?? 0,
                Y = y ?? 0,
                Z = z ?? 0,
                Clip = modernClip,
                Clip0 = c0 ?? 0,
                Clip1 = c1 ?? 0,
                Clip2 = c2 ?? 0,
                Line = item.lineno,
                Phase = GetPhase()
            }, "VIBE");
        }

        private void ProcessGps(DFLog.DFItem item, DFLog df)
        {
            double? status = GetDouble(item, df, "Status", "Fix", "FixType");
            double? sats = GetDouble(item, df, "NSats", "Sats", "Sat");
            double? hdop = GetDouble(item, df, "HDop", "HDOP");
            double? lat = GetDouble(item, df, "Lat", "Latitude");
            double? lng = GetDouble(item, df, "Lng", "Lon", "Longitude");
            double? speed = GetDouble(item, df, "Spd", "Speed", "GSpd");
            double? course = GetDouble(item, df, "GCrs", "Course", "Crs", "Yaw");
            double? altitude = GetDouble(item, df, "Alt", "GAlt", "AltMSL", "Altitude");
            double? used = GetDouble(item, df, "U", "Used", "Use");

            if (!status.HasValue && !sats.HasValue && !lat.HasValue)
                return;

            string key = item.msgtype;
            string instance = SafeInstance(item);
            if (!string.IsNullOrEmpty(instance))
                key += "[" + instance + "]";
            if (!_gps.ContainsKey(key))
                _gps[key] = new List<GpsSample>();

            AddCapped(_gps[key], new GpsSample
            {
                TimeMs = item.timems,
                Status = status,
                Sats = sats,
                Hdop = NormalizeHdop(hdop),
                Lat = NormalizeCoordinate(lat),
                Lng = NormalizeCoordinate(lng),
                Speed = speed,
                Course = NormalizeAngle(course),
                Altitude = altitude.HasValue ? NormalizeAltitude(altitude.Value) : (double?)null,
                Used = used,
                Line = item.lineno,
                Phase = GetPhase(),
                Airborne = IsAirborne()
            }, "GPS:" + key);
        }

        private void ProcessAttitude(DFLog.DFItem item, DFLog df)
        {
            double? roll = NormalizeAngle(GetDouble(item, df, "Roll", "R"));
            double? desiredRoll = NormalizeAngle(GetDouble(item, df, "DesRoll", "DRoll", "RollDes"));
            double? pitch = NormalizeAngle(GetDouble(item, df, "Pitch", "P"));
            double? desiredPitch = NormalizeAngle(GetDouble(item, df, "DesPitch", "DPitch", "PitchDes"));
            double? yaw = NormalizeAngle(GetDouble(item, df, "Yaw", "Y"));
            double? desiredYaw = NormalizeAngle(GetDouble(item, df, "DesYaw", "DYaw", "YawDes"));
            if (!roll.HasValue && !pitch.HasValue) return;

            AddCapped(_attitudes, new AttitudeSample
            {
                TimeMs = item.timems,
                Roll = roll, DesiredRoll = desiredRoll, Pitch = pitch, DesiredPitch = desiredPitch, Yaw = yaw, DesiredYaw = desiredYaw,
                RollError = roll.HasValue && desiredRoll.HasValue ? Math.Abs(Wrap180(roll.Value - desiredRoll.Value)) : (double?)null,
                PitchError = pitch.HasValue && desiredPitch.HasValue ? Math.Abs(pitch.Value - desiredPitch.Value) : (double?)null,
                YawError = yaw.HasValue && desiredYaw.HasValue ? Math.Abs(Wrap180(yaw.Value - desiredYaw.Value)) : (double?)null,
                Line = item.lineno, Phase = GetPhase(), Airborne = IsAirborne()
            }, "ATT");
            AddCapped(_attitudeTimes, item.timems, "ATT.time");
        }

        private void ProcessAhrs2(DFLog.DFItem item, DFLog df)
        {
            double? roll = NormalizeAngle(GetDouble(item, df, "Roll", "R"));
            double? pitch = NormalizeAngle(GetDouble(item, df, "Pitch", "P"));
            double? yaw = NormalizeAngle(GetDouble(item, df, "Yaw", "Y"));
            if (!roll.HasValue && !pitch.HasValue && !yaw.HasValue) return;
            AddCapped(_ahrs2, new AhrsSample { TimeMs = item.timems, Roll = roll, Pitch = pitch, Yaw = yaw, Line = item.lineno, Phase = GetPhase(), Airborne = IsAirborne() }, "AHR2");
        }

        private void ProcessRate(DFLog.DFItem item, DFLog df)
        {
            // RATE fields are defined by ArduPilot in degrees/second. DFLogBuffer already applies storage scaling.
            double? rd = GetDouble(item, df, "RDes", "RollDes", "TarR", "DR");
            double? ra = GetDouble(item, df, "R", "Roll", "ActR");
            double? pd = GetDouble(item, df, "PDes", "PitchDes", "TarP", "DP");
            double? pa = GetDouble(item, df, "P", "Pitch", "ActP");
            double? yd = GetDouble(item, df, "YDes", "YawDes", "TarY", "DY");
            double? ya = GetDouble(item, df, "Y", "Yaw", "ActY");
            if (!rd.HasValue && !ra.HasValue && !pd.HasValue && !pa.HasValue && !yd.HasValue && !ya.HasValue) return;
            AddCapped(_rates, new RateSample { TimeMs = item.timems, RollDesired = rd, RollActual = ra, PitchDesired = pd, PitchActual = pa, YawDesired = yd, YawActual = ya, Line = item.lineno, Phase = GetPhase(), Airborne = IsAirborne() }, "RATE");
        }

        private void ProcessImu(DFLog.DFItem item, DFLog df)
        {
            // Standard ArduPilot IMU GyrX/Y/Z fields are radians/second; convert explicitly to deg/s.
            double? gx = GetDouble(item, df, "GyrX", "GyroX", "GX");
            double? gy = GetDouble(item, df, "GyrY", "GyroY", "GY");
            double? gz = GetDouble(item, df, "GyrZ", "GyroZ", "GZ");
            double? ax = GetDouble(item, df, "AccX", "AX");
            double? ay = GetDouble(item, df, "AccY", "AY");
            double? az = GetDouble(item, df, "AccZ", "AZ");
            if (!gx.HasValue && !gy.HasValue && !gz.HasValue && !ax.HasValue && !ay.HasValue && !az.HasValue) return;
            const double RadToDeg = 57.29577951308232;
            AddCapped(_imu, new ImuSample
            {
                TimeMs = item.timems, Source = item.msgtype,
                GyroXDegPerSec = gx.HasValue ? gx.Value * RadToDeg : (double?)null,
                GyroYDegPerSec = gy.HasValue ? gy.Value * RadToDeg : (double?)null,
                GyroZDegPerSec = gz.HasValue ? gz.Value * RadToDeg : (double?)null,
                AccelX = ax, AccelY = ay, AccelZ = az, Line = item.lineno, Phase = GetPhase(), Airborne = IsAirborne()
            }, "IMU");
        }

        private void ProcessMissionCommand(DFLog.DFItem item, DFLog df)
        {
            int total = (int)(GetDouble(item, df, "CTot", "Total") ?? 0);
            int sequence = (int)(GetDouble(item, df, "CNum", "Seq", "Sequence") ?? 0);
            int command = (int)(GetDouble(item, df, "CId", "Command", "Cmd") ?? 0);
            double? lat = NormalizeCoordinate(GetDouble(item, df, "Lat", "Latitude"));
            double? lng = NormalizeCoordinate(GetDouble(item, df, "Lng", "Lon", "Longitude"));
            double? alt = GetDouble(item, df, "Alt", "Altitude");
            var record = new MissionCommandRecord
            {
                TimeMs = item.timems, Total = total, Sequence = sequence, Command = command,
                Param1 = GetDouble(item, df, "Prm1", "Param1"), Param2 = GetDouble(item, df, "Prm2", "Param2"),
                Param3 = GetDouble(item, df, "Prm3", "Param3"), Param4 = GetDouble(item, df, "Prm4", "Param4"),
                Lat = lat, Lng = lng, Altitude = alt, Frame = (int)(GetDouble(item, df, "Frame") ?? 0),
                Executed = string.Equals(item.msgtype, "MISE", StringComparison.OrdinalIgnoreCase),
                Line = item.lineno, Phase = GetPhase()
            };
            AddCapped(_missionCommands, record, "MissionCommands");
            if (record.Executed)
                AddTimeline(item, "Mission", "Executing mission item " + sequence.ToString(CultureInfo.InvariantCulture) + ": " + record.CommandName);
        }

        private void ProcessReferencePoint(DFLog.DFItem item, DFLog df)
        {
            double? lat = NormalizeCoordinate(GetDouble(item, df, "Lat", "Latitude"));
            double? lng = NormalizeCoordinate(GetDouble(item, df, "Lng", "Lon", "Longitude"));
            if (!lat.HasValue || !lng.HasValue) return;
            int type = (int)(GetDouble(item, df, "Type") ?? -1);
            AddCapped(_referencePoints, new ReferencePointRecord
            {
                TimeMs = item.timems, Type = type, Lat = lat.Value, Lng = lng.Value,
                Altitude = GetDouble(item, df, "Alt", "Altitude"), Line = item.lineno, Phase = GetPhase()
            }, "ReferencePoints");
        }

        private void ProcessAltitude(DFLog.DFItem item, DFLog df)
        {
            double? alt = GetDouble(item, df, "Alt", "RelAlt");
            double? desiredAlt = GetDouble(item, df, "DAlt", "DesAlt", "AltTarget");
            double? climb = GetDouble(item, df, "CRt", "Climb", "VelZ");
            double? desiredClimb = GetDouble(item, df, "DCRt", "DesCRt", "VelZTarget");

            if (alt.HasValue)
                _currentAltitude = NormalizeAltitude(alt.Value);
            if (climb.HasValue)
                _currentClimbRate = NormalizeClimb(climb.Value);

            AddCapped(_altitudes, new AltitudeSample
            {
                TimeMs = item.timems,
                Altitude = alt.HasValue ? NormalizeAltitude(alt.Value) : (double?)null,
                DesiredAltitude = desiredAlt.HasValue ? NormalizeAltitude(desiredAlt.Value) : (double?)null,
                ClimbRate = climb.HasValue ? NormalizeClimb(climb.Value) : (double?)null,
                DesiredClimbRate = desiredClimb.HasValue ? NormalizeClimb(desiredClimb.Value) : (double?)null,
                Mode = _currentMode,
                Line = item.lineno,
                Phase = GetPhase(),
                Airborne = IsAirborne()
            }, "Altitude");
        }

        private void ProcessMotor(DFLog.DFItem item, DFLog df)
        {
            var values = new Dictionary<int, double>();
            for (int i = 1; i <= 16; i++)
            {
                double? value = GetDouble(item, df, "C" + i.ToString(CultureInfo.InvariantCulture),
                    "Chan" + i.ToString(CultureInfo.InvariantCulture));
                if (value.HasValue)
                    values[i] = value.Value;
            }

            if (values.Count == 0)
                return;

            if (!_armed)
            {
                double maxOutput = values.Values.Max();
                if (maxOutput > 1100)
                    _armed = true;
            }

            AddCapped(_motors, new MotorSample
            {
                TimeMs = item.timems,
                Values = values,
                Line = item.lineno,
                Phase = GetPhase(),
                Airborne = IsAirborne()
            }, "RCOU");
        }

        private void ProcessRcInput(DFLog.DFItem item, DFLog df)
        {
            var values = new Dictionary<int, double>();
            for (int i = 1; i <= 14; i++)
            {
                double? value = GetDouble(item, df, "C" + i.ToString(CultureInfo.InvariantCulture),
                    "Chan" + i.ToString(CultureInfo.InvariantCulture));
                if (value.HasValue)
                    values[i] = value.Value;
            }
            if (values.Count == 0)
                return;

            AddCapped(_rcInputs, new RcInputSample
            {
                TimeMs = item.timems,
                Values = values,
                OverrideMask = _currentRcOverrideMask,
                Flags = _currentRcFlags,
                Line = item.lineno,
                Phase = GetPhase()
            }, "RCIN");
        }

        private void ProcessRcInput2(DFLog.DFItem item, DFLog df)
        {
            _currentRcOverrideMask = (int)(GetDouble(item, df, "OMask", "OverrideMask", "Mask") ?? _currentRcOverrideMask);
            _currentRcFlags = (int)(GetDouble(item, df, "Flags", "Flg") ?? _currentRcFlags);
            double? c15 = GetDouble(item, df, "C15", "Chan15");
            double? c16 = GetDouble(item, df, "C16", "Chan16");

            RcInputSample sample = _rcInputs.Count == 0 ? null : _rcInputs[_rcInputs.Count - 1];
            if (sample == null || Math.Abs(sample.TimeMs - item.timems) > 250)
            {
                sample = new RcInputSample
                {
                    TimeMs = item.timems,
                    Values = new Dictionary<int, double>(),
                    Line = item.lineno,
                    Phase = GetPhase()
                };
                AddCapped(_rcInputs, sample, "RCIN");
            }
            if (c15.HasValue) sample.Values[15] = c15.Value;
            if (c16.HasValue) sample.Values[16] = c16.Value;
            sample.OverrideMask = _currentRcOverrideMask;
            sample.Flags = _currentRcFlags;
        }

        private void ProcessMavCommand(DFLog.DFItem item, DFLog df)
        {
            var command = new MavCommandRecord
            {
                FilePath = _filePath,
                TimeMs = item.timems,
                Time = GetTimeText(item.timems),
                Phase = GetPhase(),
                TargetSystem = (int)(GetDouble(item, df, "TS", "TargetSystem") ?? -1),
                TargetComponent = (int)(GetDouble(item, df, "TC", "TargetComponent") ?? -1),
                SourceSystem = (int)(GetDouble(item, df, "SS", "SourceSystem") ?? -1),
                SourceComponent = (int)(GetDouble(item, df, "SC", "SourceComponent") ?? -1),
                Frame = (int)(GetDouble(item, df, "Fr", "Frm", "Frame") ?? -1),
                Command = (int)(GetDouble(item, df, "Cmd", "Command") ?? -1),
                Param1 = GetDouble(item, df, "P1", "Param1"),
                Param2 = GetDouble(item, df, "P2", "Param2"),
                Param3 = GetDouble(item, df, "P3", "Param3"),
                Param4 = GetDouble(item, df, "P4", "Param4"),
                X = GetDouble(item, df, "X"),
                Y = GetDouble(item, df, "Y"),
                Z = GetDouble(item, df, "Z"),
                Result = (int)(GetDouble(item, df, "Res", "Result") ?? -1),
                WasCommandLong = (GetDouble(item, df, "WL", "WasLong", "CommandLong") ?? 0) > 0.5,
                Line = item.lineno
            };
            command.CommandName = MavCommandDecoder.CommandName(command.Command);
            command.ResultName = MavCommandDecoder.ResultName(command.Result);
            _mavCommands.Add(command);

            if (command.Command == 400 && command.Param1.HasValue && (command.Result < 0 || command.Result == 0))
            {
                _stateEvidence.Add(new StateEvidence
                {
                    TimeMs = item.timems,
                    Kind = command.Param1.Value >= 0.5 ? "MAV_ARM" : "MAV_DISARM",
                    Value = command.Param1.Value,
                    Text = command.CommandName,
                    Line = item.lineno
                });
            }

            AddTimeline(item, "GCS CMD", command.CommandName + " from " + command.SourceText +
                " → " + command.ResultName + " (" + command.TransportText + ")");
        }

        private void FinalizeRcInputs()
        {
            int rollChannel = GetMappedRcChannel("RCMAP_ROLL", 1);
            int pitchChannel = GetMappedRcChannel("RCMAP_PITCH", 2);
            int throttleChannel = GetMappedRcChannel("RCMAP_THROTTLE", 3);
            int yawChannel = GetMappedRcChannel("RCMAP_YAW", 4);

            foreach (RcInputSample sample in _rcInputs)
            {
                sample.RollChannel = rollChannel;
                sample.PitchChannel = pitchChannel;
                sample.ThrottleChannel = throttleChannel;
                sample.YawChannel = yawChannel;
                sample.Roll = NormalizeCenteredRc(sample.GetRaw(rollChannel), rollChannel);
                sample.Pitch = NormalizeCenteredRc(sample.GetRaw(pitchChannel), pitchChannel);
                sample.Throttle = NormalizeThrottleRc(sample.GetRaw(throttleChannel), throttleChannel);
                sample.Yaw = NormalizeCenteredRc(sample.GetRaw(yawChannel), yawChannel);
                sample.PrimaryOverrideActive = IsRcChannelOverridden(sample.OverrideMask, rollChannel) ||
                    IsRcChannelOverridden(sample.OverrideMask, pitchChannel) ||
                    IsRcChannelOverridden(sample.OverrideMask, throttleChannel) ||
                    IsRcChannelOverridden(sample.OverrideMask, yawChannel);
            }
        }

        private int GetMappedRcChannel(string parameter, int fallback)
        {
            double value;
            if (_parameters.TryGetValue(parameter, out value) && value >= 1 && value <= 16)
                return (int)Math.Round(value);
            return fallback;
        }

        private double? NormalizeCenteredRc(double? raw, int channel)
        {
            if (!raw.HasValue) return null;
            double min = GetRcParameter(channel, "MIN", 1000);
            double max = GetRcParameter(channel, "MAX", 2000);
            double trim = GetRcParameter(channel, "TRIM", 1500);
            double denominator = raw.Value >= trim ? Math.Max(1, max - trim) : Math.Max(1, trim - min);
            double value = 100.0 * (raw.Value - trim) / denominator;
            if (IsRcReversed(channel)) value = -value;
            return Math.Max(-100, Math.Min(100, value));
        }

        private double? NormalizeThrottleRc(double? raw, int channel)
        {
            if (!raw.HasValue) return null;
            double min = GetRcParameter(channel, "MIN", 1000);
            double max = GetRcParameter(channel, "MAX", 2000);
            double value = 100.0 * (raw.Value - min) / Math.Max(1, max - min);
            value = Math.Max(0, Math.Min(100, value));
            if (IsRcReversed(channel)) value = 100 - value;
            return value;
        }

        private double GetRcParameter(int channel, string suffix, double fallback)
        {
            double value;
            string name = "RC" + channel.ToString(CultureInfo.InvariantCulture) + "_" + suffix;
            return _parameters.TryGetValue(name, out value) ? value : fallback;
        }

        private bool IsRcReversed(int channel)
        {
            double value;
            string modern = "RC" + channel.ToString(CultureInfo.InvariantCulture) + "_REVERSED";
            string legacy = "RC" + channel.ToString(CultureInfo.InvariantCulture) + "_REV";
            if (_parameters.TryGetValue(modern, out value))
                return value > 0.5;
            if (_parameters.TryGetValue(legacy, out value))
                return value < 0;
            return false;
        }

        private static bool IsRcChannelOverridden(int mask, int channel)
        {
            if (channel < 1 || channel > 31) return false;
            return (mask & (1 << (channel - 1))) != 0;
        }

        private void ProcessPerformance(DFLog.DFItem item, DFLog df)
        {
            TrackCounterField(item, df, "PM.NLon", true, "NLon", "LongLoops");
            TrackCounterField(item, df, "PM.I2CErr", true, "I2CErr", "I2C");
            TrackCounterField(item, df, "PM.INSErr", true, "INSErr", "INS");
            TrackCounterField(item, df, "PM.ErrL", true, "ErrL");
            TrackCounterField(item, df, "PM.ErrC", true, "ErrC");
            TrackCounterField(item, df, "PM.MaxT", false, "MaxT", "MaxTime");
            TrackCounterField(item, df, "PM.Load", false, "Load");
        }

        private void ProcessEkf(DFLog.DFItem item, DFLog df)
        {
            double? sv = GetDouble(item, df, "SV");
            double? sp = GetDouble(item, df, "SP");
            double? sh = GetDouble(item, df, "SH");
            double? sm = GetDouble(item, df, "SM");
            double? ipn = GetDouble(item, df, "IPN"); double? ipe = GetDouble(item, df, "IPE"); double? ipd = GetDouble(item, df, "IPD");
            double? ivn = GetDouble(item, df, "IVN"); double? ive = GetDouble(item, df, "IVE"); double? ivd = GetDouble(item, df, "IVD");
            double? roll = NormalizeAngle(GetDouble(item, df, "Roll", "R"));
            double? pitch = NormalizeAngle(GetDouble(item, df, "Pitch", "P"));
            int core = (int)(GetDouble(item, df, "C", "Core", "I") ?? -1);

            if (!sv.HasValue && !sp.HasValue && !sh.HasValue && !sm.HasValue &&
                !ipn.HasValue && !ipe.HasValue && !ipd.HasValue && !ivn.HasValue && !ive.HasValue && !ivd.HasValue &&
                !roll.HasValue && !pitch.HasValue) return;

            AddCapped(_ekf, new EkfSample
            {
                TimeMs = item.timems, Source = item.msgtype, Core = core, Roll = roll, Pitch = pitch,
                VelocityVariance = sv, PositionVariance = sp, HeightVariance = sh, MagneticVariance = sm,
                PositionInnovation = MaxAbs(ipn, ipe, ipd), VelocityInnovation = MaxAbs(ivn, ive, ivd),
                Line = item.lineno, Phase = GetPhase(), Airborne = IsAirborne()
            }, "EKF");
        }

        private void TrackCounterField(DFLog.DFItem item, DFLog df, string name, bool cumulative, params string[] fields)
        {
            double? value = GetDouble(item, df, fields);
            if (value.HasValue)
                TrackCounter(name, value.Value, item, cumulative);
        }

        private void TrackCounter(string name, double value, DFLog.DFItem item, bool cumulative)
        {
            if (!_counterPeaks.ContainsKey(name))
            {
                _counterPeaks[name] = new CounterPeak
                {
                    First = value,
                    Last = value,
                    Min = value,
                    Max = value,
                    TimeMs = item.timems,
                    Line = item.lineno,
                    Phase = GetPhase(),
                    Cumulative = cumulative
                };
                return;
            }

            CounterPeak counter = _counterPeaks[name];
            counter.Last = value;
            if (value > counter.Max)
            {
                counter.Max = value;
                counter.TimeMs = item.timems;
                counter.Line = item.lineno;
                counter.Phase = GetPhase();
            }
            counter.Min = Math.Min(counter.Min, value);
            _counterPeaks[name] = counter;
        }

        private void RephaseAllResolvedData()
        {
            foreach (Finding item in _result.Findings)
                item.Phase = FlightStateResolver.StateAt(_result.FlightStates, item.TimeMs);
            foreach (TimelineEvent item in _result.Timeline)
                item.Phase = FlightStateResolver.StateAt(_result.FlightStates, item.TimeMs);
            foreach (List<BatterySample> list in _batteries.Values)
                foreach (BatterySample item in list) item.Phase = FlightStateResolver.StateAt(_result.FlightStates, item.TimeMs);
            foreach (List<GpsSample> list in _gps.Values)
                foreach (GpsSample item in list)
                {
                    item.Phase = FlightStateResolver.StateAt(_result.FlightStates, item.TimeMs);
                    item.Airborne = FlightStateResolver.AirborneAt(_result.FlightStates, item.TimeMs);
                }
            foreach (VibeSample item in _vibes) item.Phase = FlightStateResolver.StateAt(_result.FlightStates, item.TimeMs);
            foreach (AttitudeSample item in _attitudes)
            {
                item.Phase = FlightStateResolver.StateAt(_result.FlightStates, item.TimeMs);
                item.Airborne = FlightStateResolver.AirborneAt(_result.FlightStates, item.TimeMs);
            }
            foreach (AltitudeSample item in _altitudes)
            {
                item.Phase = FlightStateResolver.StateAt(_result.FlightStates, item.TimeMs);
                item.Airborne = FlightStateResolver.AirborneAt(_result.FlightStates, item.TimeMs);
            }
            foreach (MotorSample item in _motors)
            {
                item.Phase = FlightStateResolver.StateAt(_result.FlightStates, item.TimeMs);
                item.Airborne = FlightStateResolver.AirborneAt(_result.FlightStates, item.TimeMs);
            }
            foreach (RcInputSample item in _rcInputs) item.Phase = FlightStateResolver.StateAt(_result.FlightStates, item.TimeMs);
            foreach (MavCommandRecord item in _mavCommands) item.Phase = FlightStateResolver.StateAt(_result.FlightStates, item.TimeMs);
            foreach (MissionCommandRecord item in _missionCommands) item.Phase = FlightStateResolver.StateAt(_result.FlightStates, item.TimeMs);
            foreach (ReferencePointRecord item in _referencePoints) item.Phase = FlightStateResolver.StateAt(_result.FlightStates, item.TimeMs);
            foreach (EkfSample item in _ekf)
            {
                item.Phase = FlightStateResolver.StateAt(_result.FlightStates, item.TimeMs);
                item.Airborne = FlightStateResolver.AirborneAt(_result.FlightStates, item.TimeMs);
            }
            foreach (AhrsSample item in _ahrs2)
            {
                item.Phase = FlightStateResolver.StateAt(_result.FlightStates, item.TimeMs);
                item.Airborne = FlightStateResolver.AirborneAt(_result.FlightStates, item.TimeMs);
            }
            foreach (ImuSample item in _imu)
            {
                item.Phase = FlightStateResolver.StateAt(_result.FlightStates, item.TimeMs);
                item.Airborne = FlightStateResolver.AirborneAt(_result.FlightStates, item.TimeMs);
            }
            foreach (RateSample item in _rates)
            {
                item.Phase = FlightStateResolver.StateAt(_result.FlightStates, item.TimeMs);
                item.Airborne = FlightStateResolver.AirborneAt(_result.FlightStates, item.TimeMs);
            }
            foreach (CounterPeak item in _counterPeaks.Values) item.Phase = FlightStateResolver.StateAt(_result.FlightStates, item.TimeMs);
        }

        private void FinalizeGeneralChecks()
        {
            _result.FirmwareMessages = _result.Timeline
                .Where(t => t.Type == "MSG")
                .Select(t => t.Text)
                .Take(20)
                .ToList();

            foreach (DataAvailabilityAssessment assessment in _result.DataAvailability)
            {
                if (assessment.Status == "Available" || assessment.Status == "Monitoring disabled" ||
                    assessment.Status == "Sensor not installed" || assessment.Status == "Configured but unused")
                    continue;

                string severity = assessment.Status == "Parser failure" ? "Warning" : "Advisory";
                AddFinding(severity, assessment.Confidence, "Data availability", assessment.Group + ": " + assessment.Status,
                    assessment.Reason, assessment.Recommendation, 0, 0, "Whole log");
            }

            if (_result.Integrity != null && _result.Integrity.Status != "Normal")
            {
                string severity = (_result.Integrity.Status == "Incomplete" || _result.Integrity.Status == "Abnormal") ? "Warning" : "Advisory";
                AddFinding(severity, _result.Integrity.Confidence, "Log integrity", "Log ending classified as " + _result.Integrity.Status,
                    _result.Integrity.Reason,
                    "Verify the raw end of the log and the vehicle's final state. Do not assume power loss unless independent power evidence supports it; file/download truncation and interrupted logging remain alternatives.",
                    _result.LastTimeMs, 0, FlightStateResolver.StateAt(_result.FlightStates, _result.LastTimeMs));
            }

            if (_result.DurationSeconds <= 1)
            {
                AddFinding("Warning", "High", "Logging", "Very short or incomplete log",
                    "Calculated log duration is only " + _result.DurationSeconds.ToString("0.0", CultureInfo.InvariantCulture) + " seconds.",
                    "Check whether the file was truncated or logging stopped unexpectedly.", 0, 0, "Whole log");
            }

            if (_result.ItemParseErrors > 0)
            {
                AddFinding("Warning", "Medium", "Log parser",
                    "Some records could not be interpreted",
                    _result.ItemParseErrors.ToString("N0", CultureInfo.InvariantCulture) + " record(s) raised parser/field exceptions during analysis." +
                    (_itemParseErrorSamples.Count > 0 ? " Examples: " + string.Join(" | ", _itemParseErrorSamples.ToArray()) : string.Empty),
                    "Treat event attribution conservatively where nearby telemetry is sparse; verify raw records around critical timestamps.",
                    _result.LastTimeMs, 0, FlightStateResolver.StateAt(_result.FlightStates, _result.LastTimeMs));
            }

            if (_result.SampleCaps.Count > 0)
            {
                string top = string.Join("; ", _result.SampleCaps.Take(4).Select(s =>
                    s.Stream + " dropped " + s.DroppedSamples.ToString("N0", CultureInfo.InvariantCulture) + " sample(s) after cap " + s.Limit.ToString("N0", CultureInfo.InvariantCulture) + "."));
                AddFinding("Advisory", "Medium", "Sampling",
                    "Long-log sample caps were reached",
                    "Some telemetry streams exceeded in-memory sample caps; early-flight density may be higher than later-flight density. " + top,
                    "For high-precision event reconstruction in long logs, verify the same window in raw Log Browse around the suspected event time.",
                    _result.LastTimeMs, 0, FlightStateResolver.StateAt(_result.FlightStates, _result.LastTimeMs));
            }
        }

        private void FinalizeBatteryChecks()
        {
            foreach (var pair in _batteries)
            {
                List<BatterySample> samples = pair.Value.Where(s => s.Voltage.HasValue && s.Voltage.Value > 0).ToList();
                if (samples.Count == 0)
                    continue;

                BatterySample minimum = samples.OrderBy(s => s.Voltage.Value).First();
                double minVoltage = NormalizeVoltage(minimum.Voltage.Value);
                double baseline = Percentile(samples.Select(s => NormalizeVoltage(s.Voltage.Value)).ToList(), 0.95);
                double sagPercent = baseline > 0 ? 100.0 * (baseline - minVoltage) / baseline : 0;
                double maxCurrent = pair.Value.Where(s => s.Current.HasValue).Select(s => Math.Abs(s.Current.Value)).DefaultIfEmpty(0).Max();
                double usedMah = pair.Value.Where(s => s.UsedMah.HasValue).Select(s => s.UsedMah.Value).DefaultIfEmpty(0).Max();

                string prefix = pair.Key.StartsWith("BAT2", StringComparison.OrdinalIgnoreCase) ? "BATT2_" : "BATT_";
                double low = GetParameter(prefix + "LOW_VOLT");
                double critical = GetParameter(prefix + "CRT_VOLT");
                double capacity = GetParameter(prefix + "CAPACITY");

                string evidence = string.Format(CultureInfo.InvariantCulture,
                    "{0}: minimum {1:0.00} V, approximate upper baseline {2:0.00} V, relative sag {3:0.0}%, maximum current {4:0.0} A, logged consumption {5:0} mAh.",
                    pair.Key, minVoltage, baseline, sagPercent, maxCurrent, usedMah);

                if (critical > 0 && minVoltage <= critical)
                {
                    AddFinding("Critical", "High", "Battery", pair.Key + " reached the configured critical voltage",
                        evidence + " Configured critical voltage is " + critical.ToString("0.00", CultureInfo.InvariantCulture) + " V.",
                        "Inspect battery condition, connector resistance, power-module calibration and load demand before further flight.",
                        minimum.TimeMs, minimum.Line, minimum.Phase);
                }
                else if (low > 0 && minVoltage <= low)
                {
                    AddFinding("Warning", "High", "Battery", pair.Key + " reached the configured low-voltage threshold",
                        evidence + " Configured low voltage is " + low.ToString("0.00", CultureInfo.InvariantCulture) + " V.",
                        "Verify battery health and confirm the failsafe threshold leaves adequate recovery margin.",
                        minimum.TimeMs, minimum.Line, minimum.Phase);
                }
                else if (sagPercent >= 20)
                {
                    AddFinding("Warning", "Medium", "Battery", pair.Key + " shows large voltage sag",
                        evidence, "Check battery internal resistance, connector heating, cable losses and current-sensor calibration. Compare against healthy flights using the same battery type.",
                        minimum.TimeMs, minimum.Line, minimum.Phase);
                }
                else if (sagPercent >= 12)
                {
                    AddFinding("Advisory", "Medium", "Battery", pair.Key + " shows noticeable voltage sag",
                        evidence, "Trend this value across flights and batteries; relative sag alone does not prove a defective battery.",
                        minimum.TimeMs, minimum.Line, minimum.Phase);
                }

                if (capacity > 0 && usedMah > capacity * 0.85)
                {
                    AddFinding(usedMah > capacity ? "Warning" : "Advisory", "High", "Battery",
                        pair.Key + " used most of the configured capacity",
                        string.Format(CultureInfo.InvariantCulture, "Logged use {0:0} mAh versus configured capacity {1:0} mAh ({2:0}%).",
                            usedMah, capacity, 100 * usedMah / capacity),
                        "Confirm the configured capacity is correct and retain a safe reserve appropriate to the aircraft and mission.",
                        pair.Value.Last().TimeMs, pair.Value.Last().Line, pair.Value.Last().Phase);
                }
            }
        }

        private void FinalizeVibrationChecks()
        {
            if (_vibes.Count == 0) return;

            // Stable-flight vibration is evaluated separately from takeoff/landing transients.
            List<VibeSample> stable = _vibes.Where(v => string.Equals(v.Phase, "Airborne", StringComparison.OrdinalIgnoreCase)).ToList();
            if (stable.Count < 10) stable = _vibes.Where(v => FlightStateResolver.IsAirborneState(v.Phase) && !string.Equals(v.Phase, "Landing", StringComparison.OrdinalIgnoreCase)).ToList();
            List<VibeSample> landing = _vibes.Where(v => string.Equals(v.Phase, "Landing", StringComparison.OrdinalIgnoreCase) || string.Equals(v.Phase, "Landed", StringComparison.OrdinalIgnoreCase)).ToList();

            if (stable.Count >= 5)
            {
                var maxima = stable.Select(v => Math.Max(v.X, Math.Max(v.Y, v.Z))).ToList();
                double p95 = Percentile(maxima, 0.95);
                VibeSample peak = stable.OrderByDescending(v => Math.Max(v.X, Math.Max(v.Y, v.Z))).First();
                double max = Math.Max(peak.X, Math.Max(peak.Y, peak.Z));
                double clipIncrease = ComputeClipIncrease(stable);
                string evidence = string.Format(CultureInfo.InvariantCulture,
                    "Stable-flight 95th-percentile vibration {0:0.0} m/s²; peak {1:0.0} m/s²; clipping increase {2:0}.", p95, max, clipIncrease);
                if (clipIncrease > 0 || p95 > 60)
                    AddFinding("Warning", "High", "Vibration", "High in-flight vibration or accelerometer clipping was recorded", evidence,
                        "Inspect propellers, motors, bearings, frame stiffness and flight-controller isolation. Correlate onset with control error and motor output before treating vibration as the initiating cause.", peak.TimeMs, peak.Line, peak.Phase);
                else if (p95 > 30 || max > 60)
                    AddFinding("Advisory", "High", "Vibration", "In-flight vibration exceeded the preferred range", evidence,
                        "Inspect mechanical balance and mounting, especially if estimator or control-response problems occurred at the same time.", peak.TimeMs, peak.Line, peak.Phase);
            }

            // Landing-only clipping is context, not automatically an in-flight vibration failure.
            if (landing.Count >= 2 && ComputeClipIncrease(landing) > 0 && (stable.Count == 0 || ComputeClipIncrease(stable) <= 0))
            {
                VibeSample item = landing[landing.Count - 1];
                AddFinding("Advisory", "High", "Vibration", "Accelerometer clipping occurred only during landing/ground-contact phase",
                    "Clipping counters increased during the resolved landing/landed phase without a corresponding in-flight clipping increase.",
                    "Treat this primarily as landing/impact evidence. Review descent rate and touchdown behavior before changing vibration isolation or tuning.", item.TimeMs, item.Line, item.Phase);
            }
        }

        private static double ComputeClipIncrease(List<VibeSample> samples)
        {
            if (samples == null || samples.Count < 2) return 0;
            double total = 0;
            foreach (IGrouping<int, VibeSample> group in samples.GroupBy(v => v.Instance))
            {
                List<VibeSample> ordered = group.OrderBy(v => v.TimeMs).ToList();
                if (ordered.Any(v => v.Clip.HasValue))
                {
                    double first = ordered.First(v => v.Clip.HasValue).Clip.Value;
                    double last = ordered.Last(v => v.Clip.HasValue).Clip.Value;
                    total += Math.Max(0, last - first);
                }
                else
                {
                    VibeSample first = ordered[0]; VibeSample last = ordered[ordered.Count - 1];
                    total += Math.Max(0, last.Clip0 - first.Clip0) + Math.Max(0, last.Clip1 - first.Clip1) + Math.Max(0, last.Clip2 - first.Clip2);
                }
            }
            return total;
        }

        private void FinalizeGpsChecks()
        {
            foreach (var pair in _gps)
            {
                if (!SensorAssessmentBuilder.ShouldEvaluateGps(_result, pair.Key))
                    continue;

                List<GpsSample> flight = pair.Value.Where(s => s.Airborne).ToList();
                if (flight.Count == 0)
                    flight = pair.Value;
                if (flight.Count == 0)
                    continue;

                List<GpsSample> badFix = flight.Where(s => s.Status.HasValue && s.Status.Value < 3).ToList();
                List<GpsSample> lowSats = flight.Where(s => s.Sats.HasValue && s.Sats.Value < 6).ToList();
                List<GpsSample> highHdop = flight.Where(s => s.Hdop.HasValue && s.Hdop.Value > 2.5).ToList();

                if (badFix.Count > 0)
                {
                    GpsSample worst = badFix.OrderBy(s => s.Status.Value).First();
                    AddFinding("Warning", "High", "GPS", pair.Key + " lost a 3D fix during the analyzed flight",
                        string.Format(CultureInfo.InvariantCulture, "{0} of {1} evaluated samples had fix status below 3; worst status {2:0}.", badFix.Count, flight.Count, worst.Status),
                        "Inspect antenna placement, RF interference, sky view, GPS power and the exact mode/EKF response around this timestamp.",
                        worst.TimeMs, worst.Line, worst.Phase);
                }

                if (lowSats.Count > flight.Count * 0.05)
                {
                    GpsSample worst = lowSats.OrderBy(s => s.Sats.Value).First();
                    AddFinding("Advisory", "High", "GPS", pair.Key + " frequently had a low satellite count",
                        string.Format(CultureInfo.InvariantCulture, "{0:0.0}% of evaluated samples were below 6 satellites; minimum {1:0}.", 100.0 * lowSats.Count / flight.Count, worst.Sats),
                        "Compare with other flights at the same installation and check antenna obstruction or interference.",
                        worst.TimeMs, worst.Line, worst.Phase);
                }

                if (highHdop.Count > flight.Count * 0.05)
                {
                    GpsSample worst = highHdop.OrderByDescending(s => s.Hdop.Value).First();
                    AddFinding("Advisory", "High", "GPS", pair.Key + " reported degraded position geometry",
                        string.Format(CultureInfo.InvariantCulture, "{0:0.0}% of evaluated samples had HDop above 2.5; maximum {1:0.00}.", 100.0 * highHdop.Count / flight.Count, worst.Hdop),
                        "Check whether EKF innovations, position jumps or mode changes occurred at the same time.",
                        worst.TimeMs, worst.Line, worst.Phase);
                }

                for (int i = 1; i < flight.Count; i++)
                {
                    GpsSample a = flight[i - 1];
                    GpsSample b = flight[i];
                    if (!a.Lat.HasValue || !a.Lng.HasValue || !b.Lat.HasValue || !b.Lng.HasValue)
                        continue;
                    double dt = (b.TimeMs - a.TimeMs) / 1000.0;
                    if (dt <= 0.05 || dt > 5)
                        continue;
                    double distance = HaversineMeters(a.Lat.Value, a.Lng.Value, b.Lat.Value, b.Lng.Value);
                    double impliedSpeed = distance / dt;
                    double loggedSpeed = Math.Max(Math.Abs(a.Speed ?? 0), Math.Abs(b.Speed ?? 0));
                    if (impliedSpeed > 150 && loggedSpeed < 70)
                    {
                        AddFinding("Warning", "Medium", "GPS", pair.Key + " contains a possible position jump",
                            string.Format(CultureInfo.InvariantCulture, "Position changed {0:0} m in {1:0.00} s (implied {2:0} m/s) while logged speed was at most {3:0} m/s.", distance, dt, impliedSpeed, loggedSpeed),
                            "Verify the coordinates and correlate with GPS status, HDop, EKF innovations and position-reset messages.",
                            b.TimeMs, b.Line, b.Phase);
                        break;
                    }
                }
            }
        }

        private void FinalizeAttitudeChecks()
        {
            List<AttitudeSample> flight = _attitudes.Where(a => a.Airborne).ToList();
            if (flight.Count == 0)
                flight = _attitudes;
            if (flight.Count == 0)
                return;

            EvaluateAngleError(flight, "roll", delegate(AttitudeSample a) { return a.RollError; }, 10, 25);
            EvaluateAngleError(flight, "pitch", delegate(AttitudeSample a) { return a.PitchError; }, 10, 25);
            EvaluateAngleError(flight, "yaw", delegate(AttitudeSample a) { return a.YawError; }, 20, 45);
        }

        private void EvaluateAngleError(List<AttitudeSample> samples, string axis,
            Func<AttitudeSample, double?> selector, double advisory, double warning)
        {
            List<AttitudeSample> valid = samples.Where(s => selector(s).HasValue).ToList();
            if (valid.Count < 20)
                return;

            double p95 = Percentile(valid.Select(s => selector(s).Value).ToList(), 0.95);
            AttitudeSample peak = valid.OrderByDescending(s => selector(s).Value).First();
            double max = selector(peak).Value;

            if (p95 > warning || max > warning * 2)
            {
                AddFinding("Warning", "Medium", "Attitude control", "Large desired-versus-actual " + axis + " error",
                    string.Format(CultureInfo.InvariantCulture, "95th-percentile error {0:0.0}°, peak {1:0.0}°.", p95, max),
                    "Check for motor saturation, wind/load limits, mechanical asymmetry, tuning issues and sensor problems at the same timestamp.",
                    peak.TimeMs, peak.Line, peak.Phase);
            }
            else if (p95 > advisory)
            {
                AddFinding("Advisory", "Medium", "Attitude control", "Elevated " + axis + " tracking error",
                    string.Format(CultureInfo.InvariantCulture, "95th-percentile error {0:0.0}°, peak {1:0.0}°.", p95, max),
                    "Compare against healthy flights in the same mode and wind conditions before changing tuning.",
                    peak.TimeMs, peak.Line, peak.Phase);
            }
        }

        private void FinalizeAltitudeChecks()
        {
            List<AltitudeSample> flight = _altitudes.Where(a => a.Airborne).ToList();
            if (flight.Count == 0)
                flight = _altitudes;
            if (flight.Count == 0)
                return;

            List<AltitudeSample> closeRapidDescent = flight.Where(a => a.Altitude.HasValue && a.ClimbRate.HasValue &&
                                                                        a.Altitude.Value >= 0 && a.Altitude.Value < 10 &&
                                                                        a.ClimbRate.Value < -3.0).ToList();
            if (closeRapidDescent.Count > 0)
            {
                AltitudeSample worst = closeRapidDescent.OrderBy(a => a.ClimbRate.Value).First();
                AddFinding("Warning", "Medium", "Vertical control", "Rapid descent was recorded close to the ground",
                    string.Format(CultureInfo.InvariantCulture, "Altitude {0:0.0} m and vertical speed {1:0.0} m/s.", worst.Altitude, worst.ClimbRate),
                    "Inspect pilot command, desired climb rate, throttle/motor saturation, rangefinder/barometer behavior and landing-detector state around this time.",
                    worst.TimeMs, worst.Line, worst.Phase);
            }

            List<AltitudeSample> altTracking = flight.Where(a => a.Altitude.HasValue && a.DesiredAltitude.HasValue &&
                                                                  Math.Abs(a.DesiredAltitude.Value) > 0.01 &&
                                                                  IsAltitudeControlledMode(a.Mode)).ToList();
            if (altTracking.Count > 30)
            {
                double p95 = Percentile(altTracking.Select(a => Math.Abs(a.Altitude.Value - a.DesiredAltitude.Value)).ToList(), 0.95);
                AltitudeSample peak = altTracking.OrderByDescending(a => Math.Abs(a.Altitude.Value - a.DesiredAltitude.Value)).First();
                double max = Math.Abs(peak.Altitude.Value - peak.DesiredAltitude.Value);
                if (p95 > 5 || max > 15)
                {
                    AddFinding("Warning", "Medium", "Vertical control", "Large altitude tracking error in an altitude-controlled mode",
                        string.Format(CultureInfo.InvariantCulture, "95th-percentile absolute error {0:0.0} m; peak {1:0.0} m in mode {2}.", p95, max, peak.Mode),
                        "Correlate with desired climb rate, motor output, battery voltage, barometer/rangefinder data and wind/load conditions.",
                        peak.TimeMs, peak.Line, peak.Phase);
                }
                else if (p95 > 2.5)
                {
                    AddFinding("Advisory", "Medium", "Vertical control", "Altitude tracking error was elevated",
                        string.Format(CultureInfo.InvariantCulture, "95th-percentile absolute error {0:0.0} m; peak {1:0.0} m in mode {2}.", p95, max, peak.Mode),
                        "Compare with healthy flights using the same mode and altitude source.",
                        peak.TimeMs, peak.Line, peak.Phase);
                }
            }
        }

        private void FinalizeMotorChecks()
        {
            List<MotorSample> flight = _motors.Where(m => m.Airborne).ToList();
            if (flight.Count < 20)
                flight = _motors;
            if (flight.Count < 20)
                return;

            var channels = flight.SelectMany(m => m.Values.Keys).Distinct().OrderBy(x => x).ToList();
            var active = new List<int>();
            foreach (int channel in channels)
            {
                List<double> values = flight.Where(m => m.Values.ContainsKey(channel)).Select(m => m.Values[channel]).ToList();
                if (values.Count < 20)
                    continue;
                double range = values.Max() - values.Min();
                double median = Percentile(values, 0.5);
                if ((median > 800 && range > 80) || (median >= 0 && median <= 1.1 && range > 0.1))
                    active.Add(channel);
            }

            if (active.Count < 3)
            {
                AddFinding("Advisory", "Medium", "Motors", "Motor channels could not be identified confidently",
                    "RCOU was present, but fewer than three channels showed a motor-like output range.",
                    "Confirm SERVOx_FUNCTION assignments and use Mission Planner Log Browse to select the actual motor outputs.",
                    flight[0].TimeMs, flight[0].Line, flight[0].Phase);
                return;
            }

            bool pwmScale = active.SelectMany(c => flight.Where(m => m.Values.ContainsKey(c)).Select(m => m.Values[c])).Max() > 10;
            double highThreshold = pwmScale ? 1900 : 0.95;
            double lowThreshold = pwmScale ? 1100 : 0.05;
            int highCount = 0;
            int lowCount = 0;
            MotorSample highPeak = null;
            MotorSample lowPeak = null;

            var deviations = active.ToDictionary(c => c, c => new List<double>());
            var signed = active.ToDictionary(c => c, c => new List<double>());

            foreach (MotorSample sample in flight)
            {
                List<double> values = active.Where(c => sample.Values.ContainsKey(c)).Select(c => sample.Values[c]).ToList();
                if (values.Count < active.Count)
                    continue;
                double mean = values.Average();
                double span = pwmScale ? Math.Max(100, mean - 1000) : Math.Max(0.05, mean);

                foreach (int channel in active)
                {
                    double value = sample.Values[channel];
                    deviations[channel].Add(Math.Abs(value - mean) / span);
                    signed[channel].Add((value - mean) / span);
                }

                if (values.Any(v => v >= highThreshold))
                {
                    highCount++;
                    if (highPeak == null) highPeak = sample;
                }
                if (values.Any(v => v <= lowThreshold))
                {
                    lowCount++;
                    if (lowPeak == null) lowPeak = sample;
                }
            }

            double highPercent = 100.0 * highCount / flight.Count;
            if (highPercent > 5)
            {
                AddFinding(highPercent > 20 ? "Warning" : "Advisory", "Medium", "Motors",
                    "Motor output frequently approached its upper limit",
                    string.Format(CultureInfo.InvariantCulture, "At least one of channels {0} was above {1:0} for {2:0.0}% of evaluated RCOU samples.",
                        string.Join(",", active), highThreshold, highPercent),
                    "Check thrust margin, weight, propeller/motor selection, battery sag, wind and attitude demand. Confirm the active channels match the frame motor outputs.",
                    highPeak == null ? 0 : highPeak.TimeMs, highPeak == null ? 0 : highPeak.Line,
                    highPeak == null ? "Flight" : highPeak.Phase);
            }

            var averageSigned = signed.Where(p => p.Value.Count > 0)
                .ToDictionary(p => p.Key, p => p.Value.Average());
            if (averageSigned.Count > 0)
            {
                var highChannel = averageSigned.OrderByDescending(p => p.Value).First();
                var lowChannel = averageSigned.OrderBy(p => p.Value).First();
                if (highChannel.Value > 0.12 || lowChannel.Value < -0.12)
                {
                    AddFinding("Advisory", "Medium", "Motors", "Persistent motor-output asymmetry was detected",
                        string.Format(CultureInfo.InvariantCulture,
                            "Relative to the per-sample motor mean, channel C{0} averaged {1:+0.0%;-0.0%;0.0%} and C{2} averaged {3:+0.0%;-0.0%;0.0%}. Active channels: {4}.",
                            highChannel.Key, highChannel.Value, lowChannel.Key, lowChannel.Value, string.Join(",", active)),
                        "Check center of gravity, arm/propeller geometry, motor/ESC condition and frame alignment. Persistent asymmetry is evidence, not proof of a failed motor.",
                        flight[flight.Count / 2].TimeMs, flight[flight.Count / 2].Line, flight[flight.Count / 2].Phase);
                }
            }
        }

        private void FinalizeEkfChecks()
        {
            List<EkfSample> flight = _ekf.Where(e => e.Airborne).ToList();
            if (flight.Count == 0)
                flight = _ekf;
            if (flight.Count == 0)
                return;

            EvaluateEkfVariance(flight, "velocity", delegate(EkfSample e) { return e.VelocityVariance; });
            EvaluateEkfVariance(flight, "position", delegate(EkfSample e) { return e.PositionVariance; });
            EvaluateEkfVariance(flight, "height", delegate(EkfSample e) { return e.HeightVariance; });
            EvaluateEkfVariance(flight, "magnetic", delegate(EkfSample e) { return e.MagneticVariance; });

            List<EkfSample> pos = flight.Where(e => e.PositionInnovation.HasValue).ToList();
            if (pos.Count > 30)
            {
                double p95 = Percentile(pos.Select(e => e.PositionInnovation.Value).ToList(), 0.95);
                EkfSample peak = pos.OrderByDescending(e => e.PositionInnovation.Value).First();
                if (p95 > 5)
                {
                    AddFinding("Advisory", "Low", "EKF", "Large position innovations were recorded",
                        string.Format(CultureInfo.InvariantCulture, "95th-percentile combined absolute position innovation {0:0.00}; peak {1:0.00} in {2}.", p95, peak.PositionInnovation, peak.Source),
                        "Interpret using the exact firmware field definitions and compare with GPS quality, vibration and estimator resets before concluding there is an EKF fault.",
                        peak.TimeMs, peak.Line, peak.Phase);
                }
            }
        }

        private void EvaluateEkfVariance(List<EkfSample> samples, string name, Func<EkfSample, double?> selector)
        {
            List<EkfSample> valid = samples.Where(s => selector(s).HasValue).ToList();
            if (valid.Count < 20)
                return;
            double p95 = Percentile(valid.Select(s => selector(s).Value).ToList(), 0.95);
            EkfSample peak = valid.OrderByDescending(s => selector(s).Value).First();
            double max = selector(peak).Value;
            if (p95 > 1.0 || max > 2.0)
            {
                AddFinding(p95 > 2.0 ? "Warning" : "Advisory", "Medium", "EKF",
                    "EKF " + name + " variance was elevated",
                    string.Format(CultureInfo.InvariantCulture, "95th percentile {0:0.00}; peak {1:0.00} in {2}.", p95, max, peak.Source),
                    "Correlate with sensor health, GPS quality, vibration, magnetic interference, source changes and EKF reset messages.",
                    peak.TimeMs, peak.Line, peak.Phase);
            }
        }

        private void FinalizePerformanceChecks()
        {
            CounterPeak counter;
            if (_counterPeaks.TryGetValue("PM.NLon", out counter) && counter.Last > counter.First)
            {
                AddFinding("Warning", "High", "Autopilot performance", "Main-loop overrun counter increased",
                    string.Format(CultureInfo.InvariantCulture, "PM.NLon increased from {0:0} to {1:0}.", counter.First, counter.Last),
                    "Check CPU load, logging rate, peripheral errors and firmware/hardware health around the peak.",
                    counter.TimeMs, counter.Line, counter.Phase);
            }

            foreach (string name in new[] { "PM.I2CErr", "PM.INSErr", "PM.ErrL", "PM.ErrC" })
            {
                if (_counterPeaks.TryGetValue(name, out counter) && counter.Last > counter.First)
                {
                    AddFinding("Warning", "High", "Autopilot performance", name + " counter increased",
                        string.Format(CultureInfo.InvariantCulture, "Counter increased from {0:0} to {1:0}.", counter.First, counter.Last),
                        "Inspect the associated bus or sensor subsystem and correlate the first increment with flight events.",
                        counter.TimeMs, counter.Line, counter.Phase);
                }
            }

            if (_counterPeaks.TryGetValue("POWR.VccMin", out counter) && counter.Min < 4.7)
            {
                AddFinding(counter.Min < 4.5 ? "Critical" : "Warning", "High", "Power supply", "Flight-controller supply voltage was low",
                    "Minimum logged Vcc was " + counter.Min.ToString("0.00", CultureInfo.InvariantCulture) + " V.",
                    "Inspect the power module, redundant power path, connectors and regulator loading before further flight.",
                    counter.TimeMs, counter.Line, counter.Phase);
            }

            if (_attitudeTimes.Count > 10)
            {
                double maxGap = 0;
                double gapTime = 0;
                for (int i = 1; i < _attitudeTimes.Count; i++)
                {
                    double gap = (_attitudeTimes[i] - _attitudeTimes[i - 1]) / 1000.0;
                    if (gap > maxGap)
                    {
                        maxGap = gap;
                        gapTime = _attitudeTimes[i];
                    }
                }
                if (maxGap > 1.0)
                {
                    AddFinding("Advisory", "Medium", "Logging", "A gap exists in the ATT message stream",
                        "Largest gap between ATT records was " + maxGap.ToString("0.00", CultureInfo.InvariantCulture) + " seconds.",
                        "Check whether logging stopped, the controller was overloaded, or ATT logging rate changed. Confirm using the raw message table.",
                        gapTime, 0, "Flight");
                }
            }
        }

        private void AddTimeline(DFLog.DFItem item, string type, string text)
        {
            _result.Timeline.Add(new TimelineEvent
            {
                FilePath = _filePath,
                FileName = Path.GetFileName(_filePath),
                TimeMs = item.timems,
                Time = GetTimeText(item.timems),
                Phase = GetPhase(),
                Type = type,
                Text = text,
                Line = item.lineno
            });
        }

        private void AddFinding(string severity, string confidence, string subsystem, string title,
            string evidence, string recommendation, double timeMs, int line, string phase)
        {
            _result.Findings.Add(new Finding
            {
                FilePath = _filePath,
                FileName = Path.GetFileName(_filePath),
                Severity = severity,
                Confidence = confidence,
                Subsystem = subsystem,
                Title = title,
                Evidence = evidence,
                Recommendation = recommendation,
                TimeMs = timeMs,
                Time = GetTimeText(timeMs),
                Line = line,
                Phase = phase,
                SeverityRank = SeverityRank(severity),
                OccurrenceCount = 1
            });
        }

        private string GetPhase()
        {
            if (!_armed && _currentAltitude < 0.5)
                return "Disarmed";
            if (_currentAltitude < 1.0)
                return _currentClimbRate < -0.5 ? "Landing" : "Ground/low";
            if (_currentClimbRate > 0.6)
                return "Climb";
            if (_currentClimbRate < -0.6)
                return "Descent";
            if (ContainsAny((_currentMode ?? string.Empty).ToUpperInvariant(), "LOITER", "POSHOLD", "BRAKE", "ALT_HOLD"))
                return "Hover/hold";
            return "Flight";
        }

        private bool IsAirborne()
        {
            return _currentAltitude > 1.0 || (_armed && _currentAltitude > 0.3);
        }

        private string GetTimeText(double timeMs)
        {
            double relative = timeMs;
            if (_firstTime != double.MaxValue && timeMs >= _firstTime)
                relative = timeMs - _firstTime;
            if (relative < 0 || double.IsNaN(relative) || double.IsInfinity(relative))
                relative = 0;
            TimeSpan span = TimeSpan.FromMilliseconds(relative);
            return string.Format(CultureInfo.InvariantCulture, "{0:00}:{1:00}:{2:00}.{3:000}",
                (int)span.TotalHours, span.Minutes, span.Seconds, span.Milliseconds);
        }

        private double GetParameter(string name)
        {
            double value;
            return _parameters.TryGetValue(name, out value) ? value : 0;
        }

        private static string Safe(string value)
        {
            return string.IsNullOrEmpty(value) ? "n/a" : value;
        }

        private static string SafeInstance(DFLog.DFItem item)
        {
            try { return item.instance; }
            catch { return string.Empty; }
        }

        private static bool ContainsAny(string text, params string[] values)
        {
            if (string.IsNullOrEmpty(text))
                return false;
            return values.Any(v => text.IndexOf(v, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private static int SeverityRank(string severity)
        {
            if (severity == "Critical") return 4;
            if (severity == "Warning") return 3;
            if (severity == "Advisory") return 2;
            return 1;
        }

        private static string GetString(DFLog.DFItem item, DFLog df, params string[] fields)
        {
            foreach (string field in fields)
            {
                int index = df.FindMessageOffset(item.msgtype, field);
                if (index >= 0 && index < item.items.Length)
                    return item.items[index];
            }
            return null;
        }

        private static double? GetDouble(DFLog.DFItem item, DFLog df, params string[] fields)
        {
            string value = GetString(item, df, fields);
            if (string.IsNullOrWhiteSpace(value))
                return null;
            double result;
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
                return result;
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, out result))
                return result;
            return null;
        }

        private void AddCapped<T>(List<T> list, T value, string stream)
        {
            if (list.Count < _sampleLimit)
            {
                list.Add(value);
                return;
            }

            SampleCapAssessment stats;
            if (!_sampleCaps.TryGetValue(stream, out stats))
            {
                stats = new SampleCapAssessment { Stream = stream, Limit = _sampleLimit };
                _sampleCaps[stream] = stats;
            }
            stats.DroppedSamples++;

            const int tailReplacementStride = 500;
            // Keep occasional late-flight representatives so the tail is not silently frozen.
            if (stats.DroppedSamples % tailReplacementStride == 0 && list.Count > 0)
            {
                list[list.Count - 1] = value;
                stats.KeptTailReplacements++;
            }
        }

        private static double Percentile(List<double> values, double percentile)
        {
            if (values == null || values.Count == 0)
                return 0;
            values.Sort();
            double position = (values.Count - 1) * percentile;
            int lower = (int)Math.Floor(position);
            int upper = (int)Math.Ceiling(position);
            if (lower == upper)
                return values[lower];
            return values[lower] + (values[upper] - values[lower]) * (position - lower);
        }

        private static double NormalizeVoltage(double value)
        {
            // DFLogBuffer/BinaryLog already applies DataFlash field scaling. Preserve engineering units.
            return value;
        }

        private static double? NormalizeHdop(double? value)
        {
            return value;
        }

        private static double? NormalizeCoordinate(double? value)
        {
            return value;
        }

        private static double? NormalizeAngle(double? value)
        {
            return value;
        }

        private static double NormalizeAltitude(double value)
        {
            return value;
        }

        private static double NormalizeClimb(double value)
        {
            return value;
        }

        private static double Wrap180(double angle)
        {
            while (angle > 180) angle -= 360;
            while (angle < -180) angle += 360;
            return angle;
        }

        private static double? MaxAbs(params double?[] values)
        {
            var valid = values.Where(v => v.HasValue).Select(v => Math.Abs(v.Value)).ToList();
            return valid.Count == 0 ? (double?)null : valid.Max();
        }

        private static bool IsAltitudeControlledMode(string mode)
        {
            string upper = (mode ?? string.Empty).ToUpperInvariant();
            return ContainsAny(upper, "ALT", "LOITER", "POS", "AUTO", "GUIDED", "RTL", "LAND", "BRAKE", "CIRCLE");
        }

        private static double HaversineMeters(double lat1, double lon1, double lat2, double lon2)
        {
            const double radius = 6371000.0;
            double p1 = lat1 * Math.PI / 180.0;
            double p2 = lat2 * Math.PI / 180.0;
            double dp = (lat2 - lat1) * Math.PI / 180.0;
            double dl = (lon2 - lon1) * Math.PI / 180.0;
            double a = Math.Sin(dp / 2) * Math.Sin(dp / 2) +
                       Math.Cos(p1) * Math.Cos(p2) * Math.Sin(dl / 2) * Math.Sin(dl / 2);
            return radius * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }
    }

    internal static class IncidentCorrelationEngine
    {
        private sealed class CommandAssessment
        {
            public string Classification = "Uncertain";
            public List<string> Supporting = new List<string>();
            public List<string> Contradicting = new List<string>();
        }

        private sealed class MotorAnalysis
        {
            public double SaturationPercent;
            public double Asymmetry;
            public bool Oscillatory;
            public string OscillationEvidence;
        }

        private sealed class ControlEffectivenessAssessment
        {
            public bool Divergent;
            public string Evidence;
        }

        public static void Run(LogAnalysisResult result)
        {
            if (result == null) return;
            CrossValidateEstimators(result);
            var incidents = new List<PrincipalIncident>();
            DetectControlIncident(result, incidents);
            DetectNavigationEstimatorIncident(result, incidents);
            DetectBatteryPowerIncident(result, incidents);
            DetectRcGcsIncident(result, incidents);
            DetectLandingIncident(result, incidents);
            DetectCollisionRolloverIncident(result, incidents);
            result.Incidents = DeduplicateIncidents(incidents).OrderByDescending(i => i.SeverityScore).ThenBy(i => i.StartTimeMs).ToList();
            DeduplicateFindings(result);
            AssociateSupportingFindings(result);
        }

        private static void CrossValidateEstimators(LogAnalysisResult result)
        {
            if (result.Attitudes.Count >= 5 && result.Ahrs2.Count >= 5)
            {
                var disagreements = new List<double>();
                double peak = 0; double peakTime = 0; int peakLine = 0;
                foreach (AttitudeSample att in result.Attitudes.Where(a => a.Airborne && (a.Roll.HasValue || a.Pitch.HasValue)).Where((a, i) => i % 4 == 0))
                {
                    AhrsSample ahr = FindNearest(result.Ahrs2, att.TimeMs, a => a.TimeMs, 350);
                    if (ahr == null) continue;
                    double roll = att.Roll.HasValue && ahr.Roll.HasValue ? Math.Abs(Wrap180(att.Roll.Value - ahr.Roll.Value)) : 0;
                    double pitch = att.Pitch.HasValue && ahr.Pitch.HasValue ? Math.Abs(att.Pitch.Value - ahr.Pitch.Value) : 0;
                    double value = Math.Max(roll, pitch);
                    disagreements.Add(value);
                    if (value > peak) { peak = value; peakTime = att.TimeMs; peakLine = att.Line; }
                }
                if (disagreements.Count >= 10)
                {
                    double p95 = PercentileCopy(disagreements, 0.95);
                    result.EstimatorAgreementP95 = p95; result.EstimatorAgreementPeak = peak;
                    if (p95 > 12 || peak > 25)
                        result.Findings.Add(NewFinding(result, p95 > 20 ? "Warning" : "Advisory", "High", "Estimator",
                            "Independent attitude solutions disagree",
                            string.Format(CultureInfo.InvariantCulture, "ATT versus AHR2 disagreement: 95th percentile {0:0.0}°, peak {1:0.0}°.", p95, peak),
                            "Inspect estimator source/core switching, resets, IMU consistency and magnetic/GPS health before attributing the motion to the airframe.", peakTime, peakLine, StateAt(result, peakTime)));
                }
            }

            List<IGrouping<int, EkfSample>> cores = result.Ekf.Where(e => e.Core >= 0 && e.Airborne && (e.Roll.HasValue || e.Pitch.HasValue))
                .GroupBy(e => e.Core).Where(g => g.Count() >= 5).OrderBy(g => g.Key).ToList();
            if (cores.Count >= 2)
            {
                List<EkfSample> reference = cores[0].OrderBy(e => e.TimeMs).ToList();
                var diffs = new List<double>(); double peak = 0; double peakTime = 0; int peakLine = 0;
                for (int c = 1; c < cores.Count; c++)
                {
                    List<EkfSample> compare = cores[c].OrderBy(e => e.TimeMs).ToList();
                    foreach (EkfSample a in reference.Where((e, i) => i % 4 == 0))
                    {
                        EkfSample b = FindNearest(compare, a.TimeMs, e => e.TimeMs, 350);
                        if (b == null) continue;
                        double roll = a.Roll.HasValue && b.Roll.HasValue ? Math.Abs(Wrap180(a.Roll.Value - b.Roll.Value)) : 0;
                        double pitch = a.Pitch.HasValue && b.Pitch.HasValue ? Math.Abs(a.Pitch.Value - b.Pitch.Value) : 0;
                        double v = Math.Max(roll, pitch); diffs.Add(v);
                        if (v > peak) { peak = v; peakTime = a.TimeMs; peakLine = a.Line; }
                    }
                }
                if (diffs.Count >= 10)
                {
                    double p95 = PercentileCopy(diffs, 0.95); result.EkfCoreAgreementP95 = p95; result.EkfCoreAgreementPeak = peak;
                    if (p95 > 10 || peak > 22)
                        result.Findings.Add(NewFinding(result, p95 > 18 ? "Warning" : "Advisory", "High", "Estimator",
                            "Multiple EKF cores diverged",
                            string.Format(CultureInfo.InvariantCulture, "Cross-core attitude disagreement: 95th percentile {0:0.0}°, peak {1:0.0}° across {2} logged cores.", p95, peak, cores.Count),
                            "Review core/source selection, resets and the sensors feeding each core before assigning a physical cause.", peakTime, peakLine, StateAt(result, peakTime)));
                }
            }
        }

        private static void DetectControlIncident(LogAnalysisResult result, List<PrincipalIncident> incidents)
        {
            List<AttitudeSample> flight = result.Attitudes.Where(a => a.Airborne && (a.RollError.HasValue || a.PitchError.HasValue)).OrderBy(a => a.TimeMs).ToList();
            if (flight.Count < 10) return;
            int peakIndex = 0; double peakError = -1;
            for (int i = 0; i < flight.Count; i++) { double v = Math.Max(flight[i].RollError ?? 0, flight[i].PitchError ?? 0); if (v > peakError) { peakError = v; peakIndex = i; } }
            if (peakError < 20) return;
            AttitudeSample peak = flight[peakIndex];
            int onsetIndex = peakIndex;
            for (int i = peakIndex - 1; i >= 0; i--)
            {
                double err = Math.Max(flight[i].RollError ?? 0, flight[i].PitchError ?? 0);
                if (flight[i + 1].TimeMs - flight[i].TimeMs > 1500 || err < 8) break;
                onsetIndex = i;
            }
            int endIndex = peakIndex; int recovered = 0;
            for (int i = peakIndex + 1; i < flight.Count; i++)
            {
                if (flight[i].TimeMs - flight[i - 1].TimeMs > 2000) break;
                double err = Math.Max(flight[i].RollError ?? 0, flight[i].PitchError ?? 0); endIndex = i;
                recovered = err < 8 ? recovered + 1 : 0; if (recovered >= 3 || flight[i].TimeMs - peak.TimeMs > 8000) break;
            }
            double onset = flight[onsetIndex].TimeMs; double end = flight[endIndex].TimeMs;
            PrincipalIncident incident = BaseIncident(result, "Control", "Control response / attitude divergence", onset, peak.TimeMs, end, peak.Line);
            incident.WhatHappened = "Desired-versus-actual attitude error grew to " + peakError.ToString("0.0", CultureInfo.InvariantCulture) + "°.";
            incident.Known.Add(incident.WhatHappened);
            incident.SupportingEvidence.Add("Peak attitude tracking error " + peakError.ToString("0.0", CultureInfo.InvariantCulture) + "° at " + FormatTime(result, peak.TimeMs) + ".");

            CommandAssessment command = ClassifyCommand(result, onset, peak.TimeMs);
            incident.CommandClassification = command.Classification; incident.SupportingEvidence.AddRange(command.Supporting); incident.ContradictingEvidence.AddRange(command.Contradicting);
            MotorAnalysis motor = AnalyzeMotorsNear(result, onset, end);
            if (motor.SaturationPercent > 5) { incident.SupportingEvidence.Add("Commanded motor outputs approached saturation in " + motor.SaturationPercent.ToString("0.0", CultureInfo.InvariantCulture) + "% of nearby samples."); incident.StronglyInferred.Add("Available control authority was heavily demanded or limited during the event."); }
            if (motor.Asymmetry > 0.18) incident.SupportingEvidence.Add("Persistent motor-command asymmetry was present around the incident (relative spread " + motor.Asymmetry.ToString("0.00", CultureInfo.InvariantCulture) + ").");
            if (motor.Oscillatory) incident.SupportingEvidence.Add(motor.OscillationEvidence);
            ControlEffectivenessAssessment response = AnalyzeControlEffectiveness(result, onset, end);
            if (!string.IsNullOrEmpty(response.Evidence)) incident.SupportingEvidence.Add(response.Evidence);
            if (response.Divergent) incident.StronglyInferred.Add("Controller demand and measured angular response diverged during the event window.");

            bool measuredMotorTelemetry = result.MessageCounts.Keys.Any(k => k.StartsWith("ESC", StringComparison.OrdinalIgnoreCase) || k.StartsWith("RPM", StringComparison.OrdinalIgnoreCase));
            if (!measuredMotorTelemetry) incident.UnavailableData.Add("No ESC/RPM telemetry was logged, so RCOU proves commanded output only; physical motor/propeller response cannot be verified.");
            else incident.UnavailableData.Add("ESC/RPM records exist, but v0.5.6 does not yet pair every measured motor channel to RCOU automatically; verify physical response in Log Browse.");

            AddCauseTimingContext(result, incident);
            if (command.Classification == "Uncommanded")
            {
                incident.Title = "Probable uncommanded attitude divergence";
                incident.PossibleCauses.Add("Loss of control effectiveness from propulsion, actuator/mechanical, aerodynamic, or control-loop problems.");
                incident.PossibleCauses.Add("Estimator/control error if independent attitude solutions also disagree near onset.");
            }
            else if (command.Classification == "Pilot-commanded")
            {
                incident.Title = "Commanded aggressive maneuver with elevated tracking error";
                incident.ContradictingEvidence.Add("Pilot input supports an intentional maneuver, reducing confidence that the initial attitude change was uncommanded.");
                incident.PossibleCauses.Add("Insufficient control authority or tuning margin while following a pilot command.");
            }
            else incident.PossibleCauses.Add("Control authority, tuning, mechanical asymmetry, estimator error, or an external disturbance; command attribution remains limited by available data.");
            incident.LikelyConsequences = "Reduced attitude-control margin; persistence at low altitude can lead to impact or rollover.";
            incident.Recommendations.Add("Inspect at least 5 s before onset through recovery, comparing pilot/GCS demand, desired/actual attitude or rates, and motor output.");
            incident.Recommendations.Add("Inspect propulsion/mechanics before changing tuning solely from this log.");
            ApplyCoreTelemetryLimitations(result, incident);
            ScoreIncident(result, incident, 4 + (command.Classification == "Uncommanded" ? 2 : 0) + (motor.SaturationPercent > 5 ? 1 : 0)); incidents.Add(incident);
        }

        private static void DetectNavigationEstimatorIncident(LogAnalysisResult result, List<PrincipalIncident> incidents)
        {
            List<Finding> nav = result.Findings.Where(f => (ContainsAny(f.Subsystem, "GPS", "EKF", "Estimator") || ContainsAny(f.Title, "GPS", "EKF", "position", "estimator")) && f.SeverityRank >= 3).OrderBy(f => f.TimeMs).ToList();
            if (nav.Count == 0) return;
            Finding anchor = nav[0];
            List<Finding> nearby = nav.Where(f => Math.Abs(f.TimeMs - anchor.TimeMs) <= 5000).ToList();
            bool gps = nearby.Any(f => ContainsAny(f.Subsystem, "GPS") || ContainsAny(f.Title, "GPS", "position jump"));
            bool ekf = nearby.Any(f => ContainsAny(f.Subsystem, "EKF", "Estimator") || ContainsAny(f.Title, "EKF", "estimator"));
            if (!gps && !ekf) return;
            PrincipalIncident incident = BaseIncident(result, gps && ekf ? "Navigation/estimator" : (gps ? "Navigation" : "Estimator"),
                gps && ekf ? "Correlated GPS/EKF navigation degradation" : (gps ? "GPS/navigation degradation" : "Estimator degradation"),
                nearby.Min(f => f.TimeMs), nearby.OrderByDescending(f => f.SeverityRank).First().TimeMs, Math.Min(result.LastTimeMs, nearby.Max(f => f.TimeMs) + 3000), anchor.Line);
            foreach (Finding f in nearby) { incident.Known.Add(f.Title + ": " + f.Evidence); incident.SupportingEvidence.Add(f.Title + " at " + f.Time + "."); }
            if (gps && ekf) incident.StronglyInferred.Add("Navigation-source and estimator evidence degraded in the same time window, making a correlated navigation event more likely than an isolated threshold excursion.");
            CommandAssessment cmd = ClassifyCommand(result, incident.StartTimeMs, incident.PeakTimeMs); incident.CommandClassification = cmd.Classification; incident.SupportingEvidence.AddRange(cmd.Supporting); incident.ContradictingEvidence.AddRange(cmd.Contradicting);
            incident.PossibleCauses.Add(gps ? "GNSS signal/fix degradation, antenna/RF environment, or position discontinuity." : "Estimator source/core inconsistency or sensor-estimation problem.");
            if (ekf) incident.PossibleCauses.Add("Sensor-source inconsistency, estimator reset/switch, vibration/magnetic interference, or genuine high dynamics.");
            if (result.Imu.Count == 0) incident.UnavailableData.Add("IMU gyro data was unavailable for physical-motion cross-validation.");
            incident.LikelyConsequences = "Navigation error, estimator fallback/reset, unexpected mode behavior, or reduced position-control performance.";
            incident.Recommendations.Add("Inspect GPS status/HDOP/satellites, EKF innovations/variances, estimator resets/source changes and mode changes in the same window.");
            ApplyCoreTelemetryLimitations(result, incident);
            ScoreIncident(result, incident, gps && ekf ? 5 : 3); incidents.Add(incident);
        }

        private static void DetectBatteryPowerIncident(LogAnalysisResult result, List<PrincipalIncident> incidents)
        {
            Finding battery = result.Findings.Where(f => f.Subsystem == "Battery" && f.SeverityRank >= 3 && FlightStateResolver.IsAirborneState(f.Phase)).OrderByDescending(f => f.SeverityRank).ThenBy(f => f.TimeMs).FirstOrDefault();
            if (battery == null) return;
            PrincipalIncident incident = BaseIncident(result, "Power/Battery", "Battery/power event", Math.Max(result.FirstTimeMs, battery.TimeMs - 1500), battery.TimeMs, Math.Min(result.LastTimeMs, battery.TimeMs + 2500), battery.Line);
            incident.Known.Add(battery.Title + ": " + battery.Evidence); incident.SupportingEvidence.Add(battery.Evidence);
            double pre = NearbyBatteryMedian(result, battery.TimeMs - 3000, battery.TimeMs - 1000); double eventV = NearbyBatteryMinimum(result, battery.TimeMs - 700, battery.TimeMs + 700);
            if (pre > 0 && eventV > 0) incident.SupportingEvidence.Add("Voltage near event was " + eventV.ToString("0.00", CultureInfo.InvariantCulture) + " V versus pre-event median " + pre.ToString("0.00", CultureInfo.InvariantCulture) + " V.");
            incident.PossibleCauses.Add("Battery state/internal resistance, connector/cabling loss, current demand, or monitor calibration depending on the configured thresholds and load context.");
            incident.ContradictingEvidence.Add("A voltage change can also follow a sudden load/control event; timing relative to the principal control event is required before calling it the root cause.");
            incident.LikelyConsequences = "Reduced propulsion/control margin or a battery failsafe if the voltage event was sufficiently severe.";
            incident.Recommendations.Add("Compare voltage and current before versus after incident onset; inspect connectors, power module and battery condition before attributing causality.");
            ApplyCoreTelemetryLimitations(result, incident);
            ScoreIncident(result, incident, 3); incidents.Add(incident);
        }

        private static void DetectRcGcsIncident(LogAnalysisResult result, List<PrincipalIncident> incidents)
        {
            TimelineEvent fs = result.Timeline.FirstOrDefault(t => ContainsAny(t.Text, "RC FAILSAFE", "RADIO FAILSAFE", "GCS FAILSAFE", "FAILSAFE_RADIO", "FAILSAFE_GCS"));
            if (fs == null) return;
            PrincipalIncident incident = BaseIncident(result, "RC/GCS", "RC or GCS failsafe / command-path event", fs.TimeMs, fs.TimeMs, Math.Min(result.LastTimeMs, fs.TimeMs + 4000), fs.Line);
            incident.CommandClassification = "Failsafe-commanded"; incident.Known.Add(fs.Text); incident.SupportingEvidence.Add("Failsafe-related log event/message at " + fs.Time + ": " + fs.Text + ".");
            RcInputSample rc = FindNearest(result.RcInputs, fs.TimeMs, r => r.TimeMs, 1500);
            if (rc != null) incident.ContradictingEvidence.Add("RCIN data remained available near the failsafe timestamp; this does not by itself prove RF link continuity because RCIN is the autopilot-side input stream.");
            incident.PossibleCauses.Add("RC receiver/link loss, GCS heartbeat/link timeout, configured failsafe logic, or command-path interruption depending on the exact failsafe source.");
            incident.LikelyConsequences = "Mode or navigation action commanded by failsafe logic rather than by the pilot at that instant.";
            incident.Recommendations.Add("Verify the exact failsafe source, configured actions and link-quality/receiver logs if available; do not infer pilot error from the failsafe action itself.");
            ApplyCoreTelemetryLimitations(result, incident);
            ScoreIncident(result, incident, 4); incidents.Add(incident);
        }

        private static void DetectLandingIncident(LogAnalysisResult result, List<PrincipalIncident> incidents)
        {
            List<AltitudeSample> landing = result.Altitudes.Where(a => string.Equals(a.Phase, "Landing", StringComparison.OrdinalIgnoreCase) && a.ClimbRate.HasValue).ToList();
            if (landing.Count == 0) return;
            AltitudeSample worst = landing.OrderBy(a => a.ClimbRate.Value).First();
            double descent = Math.Abs(Math.Min(0, worst.ClimbRate.Value));
            double clips = ClipIncreaseWindow(result, worst.TimeMs - 2500, worst.TimeMs + 3000);
            double vibe = result.Vibes.Where(v => v.TimeMs >= worst.TimeMs - 2000 && v.TimeMs <= worst.TimeMs + 3000).Select(v => Math.Max(v.X, Math.Max(v.Y, v.Z))).DefaultIfEmpty(0).Max();
            if (descent < 2.5 && clips <= 0 && vibe < 55) return;
            PrincipalIncident incident = BaseIncident(result, "Landing", "Rough landing / ground-contact event", Math.Max(result.FirstTimeMs, worst.TimeMs - 2000), worst.TimeMs, Math.Min(result.LastTimeMs, worst.TimeMs + 3500), worst.Line);
            incident.Known.Add("Resolved phase was Landing with descent rate approximately " + descent.ToString("0.0", CultureInfo.InvariantCulture) + " m/s.");
            incident.SupportingEvidence.Add("Landing descent rate " + descent.ToString("0.0", CultureInfo.InvariantCulture) + " m/s.");
            if (clips > 0) incident.SupportingEvidence.Add("Accelerometer clipping increased by " + clips.ToString("0", CultureInfo.InvariantCulture) + " in the landing/contact window.");
            if (vibe > 0) incident.SupportingEvidence.Add("Peak vibration in the landing/contact window was " + vibe.ToString("0.0", CultureInfo.InvariantCulture) + " m/s².");
            incident.CommandClassification = ClassifyCommand(result, incident.StartTimeMs, incident.PeakTimeMs).Classification;
            incident.StronglyInferred.Add("The vibration/clipping evidence is temporally associated with landing and is treated primarily as consequence/contact evidence, not automatically as the initiating cause.");
            incident.PossibleCauses.Add("High descent rate, hard touchdown, uneven surface, landing-gear interaction, or loss of control immediately before contact.");
            incident.LikelyConsequences = "Elevated landing loads and possible mechanical damage depending on vehicle type and surface.";
            incident.Recommendations.Add("Inspect descent command versus actual descent, landing detector state and mechanical condition; review several seconds before touchdown for any preceding control event.");
            ApplyCoreTelemetryLimitations(result, incident);
            ScoreIncident(result, incident, 4); incidents.Add(incident);
        }

        private static void DetectCollisionRolloverIncident(LogAnalysisResult result, List<PrincipalIncident> incidents)
        {
            AttitudeSample extreme = result.Attitudes.Where(a => a.Airborne && (a.Roll.HasValue || a.Pitch.HasValue))
                .OrderByDescending(a => Math.Max(Math.Abs(a.Roll ?? 0), Math.Abs(a.Pitch ?? 0))).FirstOrDefault();
            if (extreme == null) return;
            double angle = Math.Max(Math.Abs(extreme.Roll ?? 0), Math.Abs(extreme.Pitch ?? 0)); if (angle < 70) return;
            AltitudeSample alt = FindNearest(result.Altitudes, extreme.TimeMs, a => a.TimeMs, 1200);
            bool nearGround = alt != null && alt.Altitude.HasValue && alt.Altitude.Value < 10;
            double gyro = result.Imu.Where(i => Math.Abs(i.TimeMs - extreme.TimeMs) <= 1200).Select(i => Math.Max(Math.Abs(i.GyroXDegPerSec ?? 0), Math.Max(Math.Abs(i.GyroYDegPerSec ?? 0), Math.Abs(i.GyroZDegPerSec ?? 0)))).DefaultIfEmpty(0).Max();
            double clips = ClipIncreaseWindow(result, extreme.TimeMs - 1500, extreme.TimeMs + 2000);
            bool terminal = result.LastTimeMs - extreme.TimeMs < 2500 || StateAt(result, result.LastTimeMs) == "Abnormal or uncertain ending";
            int corroborators = (nearGround ? 1 : 0) + (gyro > 180 ? 1 : 0) + (clips > 0 ? 1 : 0) + (terminal ? 1 : 0);
            if (corroborators < 2) return;
            PrincipalIncident incident = BaseIncident(result, "Impact/Rollover", "Collision, rollover, or severe terminal-motion evidence", Math.Max(result.FirstTimeMs, extreme.TimeMs - 1500), extreme.TimeMs, Math.Min(result.LastTimeMs, extreme.TimeMs + 2500), extreme.Line);
            incident.Known.Add("Absolute roll/pitch reached approximately " + angle.ToString("0.0", CultureInfo.InvariantCulture) + "°."); incident.SupportingEvidence.Add("Extreme attitude " + angle.ToString("0.0", CultureInfo.InvariantCulture) + "° at " + FormatTime(result, extreme.TimeMs) + ".");
            if (nearGround) incident.SupportingEvidence.Add("Altitude evidence placed the event near the ground (about " + alt.Altitude.Value.ToString("0.0", CultureInfo.InvariantCulture) + " m).");
            if (gyro > 180) incident.SupportingEvidence.Add("IMU angular rate exceeded approximately " + gyro.ToString("0", CultureInfo.InvariantCulture) + " deg/s near the event.");
            if (clips > 0) incident.SupportingEvidence.Add("Accelerometer clipping increased by " + clips.ToString("0", CultureInfo.InvariantCulture) + " near the event.");
            if (terminal) incident.SupportingEvidence.Add("The event was close to the log ending or an abnormal/uncertain resolved ending.");
            CommandAssessment cmd = ClassifyCommand(result, incident.StartTimeMs, incident.PeakTimeMs); incident.CommandClassification = cmd.Classification; incident.SupportingEvidence.AddRange(cmd.Supporting); incident.ContradictingEvidence.AddRange(cmd.Contradicting);
            incident.StronglyInferred.Add("Multiple independent signals are consistent with severe physical motion/contact; this is stronger evidence than attitude magnitude alone.");
            incident.PossibleCauses.Add("Ground/obstacle contact or rollover after descent/loss of control."); incident.PossibleCauses.Add("A preceding control, propulsion, navigation/estimator, or commanded maneuver event; inspect earlier onset before treating impact evidence as root cause.");
            incident.LikelyConsequences = "Possible ground/obstacle contact, rollover or termination of controlled flight."; incident.Recommendations.Add("Prioritize the 5–10 s before this event for root-cause analysis; impact vibration/clipping that begins afterward is consequence evidence.");
            ApplyCoreTelemetryLimitations(result, incident);
            ScoreIncident(result, incident, 5 + corroborators); incidents.Add(incident);
        }

        private static CommandAssessment ClassifyCommand(LogAnalysisResult result, double start, double peak)
        {
            CommandAssessment a = new CommandAssessment(); double center = (start + peak) / 2.0;
            bool failsafe = result.Timeline.Any(t => Math.Abs(t.TimeMs - center) < 2500 && ContainsAny(t.Text, "FAILSAFE", "FS_"));
            if (failsafe) { a.Classification = "Failsafe-commanded"; a.Supporting.Add("A failsafe event/message was recorded close to maneuver onset."); return a; }
            RcInputSample rc = FindNearest(result.RcInputs, center, r => r.TimeMs, 1200);
            AttitudeSample att = FindNearest(result.Attitudes, peak, x => x.TimeMs, 600);
            string mode = ModeAt(result, center);
            double stick = rc == null ? 0 : Math.Max(Math.Abs(rc.Roll ?? 0), Math.Abs(rc.Pitch ?? 0));
            double desired = att == null ? 0 : Math.Max(Math.Abs(att.DesiredRoll ?? 0), Math.Abs(att.DesiredPitch ?? 0));
            double actual = att == null ? 0 : Math.Max(Math.Abs(att.Roll ?? 0), Math.Abs(att.Pitch ?? 0));
            List<MavCommandRecord> mav = result.MavCommands.Where(c => c.TimeMs >= start - 2500 && c.TimeMs <= peak + 1500).ToList();
            if (rc == null) a.Contradicting.Add("No RCIN sample was available near maneuver onset.");
            if (att == null) a.Contradicting.Add("No ATT sample was available near the event peak.");
            if (mav.Count == 0) a.Contradicting.Add("No nearby executed MAVLink command was logged, limiting command-path attribution.");
            if (rc != null && stick >= 50)
            {
                a.Classification = rc.PrimaryOverrideActive ? "Autopilot/GCS-commanded" : "Pilot-commanded";
                a.Supporting.Add("Roll/pitch input reached about " + stick.ToString("0", CultureInfo.InvariantCulture) + "% near onset" + (rc.PrimaryOverrideActive ? " with MAVLink RC override active." : "."));
            }
            else if (desired >= 18 && IsAutonomousMode(mode))
            {
                a.Classification = mav.Count > 0 ? "Autopilot/GCS-commanded" : "Autopilot-commanded";
                a.Supporting.Add("Desired attitude reached " + desired.ToString("0.0", CultureInfo.InvariantCulture) + "° in mode " + mode + " while available pilot-stick demand was low/unknown.");
                if (mav.Count > 0) a.Supporting.Add(mav.Count.ToString(CultureInfo.InvariantCulture) + " executed MAVLink command(s) were logged nearby; this supports GCS/autopilot context but does not prove one command requested the maneuver.");
            }
            else if (att != null && actual >= 25 && desired < 10 && stick < 25)
            {
                a.Classification = "Uncommanded"; a.Supporting.Add("Actual attitude became large while desired attitude and available pilot input remained comparatively small.");
            }
            else a.Contradicting.Add("Command sources are incomplete or mixed, so pilot/autopilot/hardware responsibility cannot be assigned confidently.");

            return a;
        }

        private static ControlEffectivenessAssessment AnalyzeControlEffectiveness(LogAnalysisResult result, double start, double end)
        {
            ControlEffectivenessAssessment o = new ControlEffectivenessAssessment();
            List<RateSample> samples = result.Rates.Where(r => r.TimeMs >= start && r.TimeMs <= end && r.Airborne).OrderBy(r => r.TimeMs).ToList();
            if (samples.Count < 8) { o.Evidence = "RATE data was unavailable or insufficient for detailed control-effectiveness analysis; attitude tracking and motor-command evidence were used instead."; return o; }
            var errors = new List<double>(); var early = new List<double>(); var late = new List<double>(); int opposite = 0; int commands = 0; int reversals = 0; double? previousSigned = null;
            for (int i = 0; i < samples.Count; i++)
            {
                double best = 0; double signed = 0; bool have = false;
                foreach (Tuple<double?, double?> pair in new[] { Tuple.Create(samples[i].RollDesired, samples[i].RollActual), Tuple.Create(samples[i].PitchDesired, samples[i].PitchActual), Tuple.Create(samples[i].YawDesired, samples[i].YawActual) })
                {
                    if (!pair.Item1.HasValue || !pair.Item2.HasValue) continue; double e = pair.Item1.Value - pair.Item2.Value; if (!have || Math.Abs(e) > Math.Abs(signed)) { signed = e; best = Math.Abs(e); have = true; }
                    if (Math.Abs(pair.Item1.Value) > 30) { commands++; if (Math.Sign(pair.Item1.Value) != 0 && Math.Sign(pair.Item2.Value) != 0 && Math.Sign(pair.Item1.Value) != Math.Sign(pair.Item2.Value) && Math.Abs(pair.Item2.Value) > 15) opposite++; }
                }
                if (!have) continue; errors.Add(best); if (i < samples.Count / 3) early.Add(best); if (i >= samples.Count * 2 / 3) late.Add(best);
                if (previousSigned.HasValue && Math.Abs(previousSigned.Value) > 10 && Math.Abs(signed) > 10 && Math.Sign(previousSigned.Value) != Math.Sign(signed)) reversals++; previousSigned = signed;
            }
            if (errors.Count < 6) return o;
            double p95 = PercentileCopy(errors, 0.95); double earlyMean = early.Count == 0 ? 0 : early.Average(); double lateMean = late.Count == 0 ? 0 : late.Average();
            o.Divergent = lateMean > Math.Max(20, earlyMean * 1.6) || (commands >= 3 && opposite >= Math.Max(2, commands / 3));
            o.Evidence = string.Format(CultureInfo.InvariantCulture, "RATE response analysis: 95th-percentile rate error {0:0.0} deg/s; early mean {1:0.0}, late mean {2:0.0}; significant error sign reversals {3}; opposite-sign responses {4}/{5} significant command axes.", p95, earlyMean, lateMean, reversals, opposite, commands);
            return o;
        }

        private static MotorAnalysis AnalyzeMotorsNear(LogAnalysisResult result, double start, double end)
        {
            MotorAnalysis o = new MotorAnalysis(); List<MotorSample> samples = result.Motors.Where(m => m.TimeMs >= start && m.TimeMs <= end && m.Airborne && m.Values != null && m.Values.Count >= 3).OrderBy(m => m.TimeMs).ToList();
            if (samples.Count < 5) return o;
            List<int> channels = samples.SelectMany(x => x.Values.Keys).Distinct().Where(ch => samples.Count(x => x.Values.ContainsKey(ch)) >= samples.Count * 0.7).OrderBy(ch => ch).Take(12).ToList(); if (channels.Count < 3) return o;
            bool pwm = samples.SelectMany(x => x.Values.Where(p => channels.Contains(p.Key)).Select(p => p.Value)).Any(v => v > 10); double high = pwm ? 1900 : 0.95; int saturated = 0; var spreads = new List<double>(); var dev = channels.ToDictionary(ch => ch, ch => new List<double>());
            foreach (MotorSample sample in samples)
            {
                List<double> vals = channels.Select(ch => sample.Values[ch]).ToList(); if (vals.Any(v => v >= high)) saturated++; double mean = vals.Average(); double scale = pwm ? Math.Max(100, mean - 1000) : Math.Max(0.05, mean); spreads.Add((vals.Max() - vals.Min()) / scale); foreach (int ch in channels) dev[ch].Add((sample.Values[ch] - mean) / scale);
            }
            o.SaturationPercent = 100.0 * saturated / samples.Count; o.Asymmetry = PercentileCopy(spreads, 0.75);
            double best = 0; int ca = -1, cb = -1;
            for (int a = 0; a < channels.Count; a++) for (int b = a + 1; b < channels.Count; b++) { double corr = Pearson(dev[channels[a]], dev[channels[b]]); double amplitude = dev[channels[a]].Zip(dev[channels[b]], (x,y)=>(Math.Abs(x)+Math.Abs(y))/2).DefaultIfEmpty(0).Average(); if (amplitude > 0.08 && corr < best) { best = corr; ca = channels[a]; cb = channels[b]; } }
            if (best < -0.65) { o.Oscillatory = true; o.OscillationEvidence = "Strong opposed/diagonal-like commanded-output oscillation was present between C" + ca + " and C" + cb + " (correlation " + best.ToString("0.00", CultureInfo.InvariantCulture) + ")."; }
            return o;
        }

        private static void AddCauseTimingContext(LogAnalysisResult result, PrincipalIncident incident)
        {
            double preVibe = result.Vibes.Where(v => v.TimeMs >= incident.StartTimeMs - 3000 && v.TimeMs < incident.StartTimeMs).Select(v => Math.Max(v.X, Math.Max(v.Y, v.Z))).DefaultIfEmpty(0).Max();
            double postVibe = result.Vibes.Where(v => v.TimeMs >= incident.StartTimeMs && v.TimeMs <= incident.PeakTimeMs + 1500).Select(v => Math.Max(v.X, Math.Max(v.Y, v.Z))).DefaultIfEmpty(0).Max();
            if (postVibe > 45 && preVibe < 25) incident.ContradictingEvidence.Add("Vibration rose mainly after control-error onset (pre-onset peak " + preVibe.ToString("0.0", CultureInfo.InvariantCulture) + ", event/post-onset peak " + postVibe.ToString("0.0", CultureInfo.InvariantCulture) + " m/s²), so it may be a consequence rather than the root cause.");
            else if (preVibe > 45) incident.SupportingEvidence.Add("High vibration was already present before control-error onset (pre-onset peak " + preVibe.ToString("0.0", CultureInfo.InvariantCulture) + " m/s²), so it remains a plausible contributing factor.");
            double preV = NearbyBatteryMedian(result, incident.StartTimeMs - 3500, incident.StartTimeMs - 1000); double eventV = NearbyBatteryMinimum(result, incident.StartTimeMs - 400, incident.PeakTimeMs + 400);
            if (preV > 0 && eventV > 0)
            {
                double drop = 100.0 * (preV - eventV) / preV;
                if (drop > 12) incident.SupportingEvidence.Add("Battery voltage fell about " + drop.ToString("0.0", CultureInfo.InvariantCulture) + "% from the pre-onset median into the event window; timing makes power/load interaction relevant.");
            }
        }

        private static double NearbyBatteryMedian(LogAnalysisResult result, double start, double end)
        { var v = result.Batteries.Values.SelectMany(x=>x).Where(x=>x.TimeMs>=start&&x.TimeMs<=end&&x.Voltage.HasValue&&x.Voltage.Value>0).Select(x=>x.Voltage.Value).ToList(); return v.Count==0?0:PercentileCopy(v,0.5); }
        private static double NearbyBatteryMinimum(LogAnalysisResult result, double start, double end)
        { return result.Batteries.Values.SelectMany(x=>x).Where(x=>x.TimeMs>=start&&x.TimeMs<=end&&x.Voltage.HasValue&&x.Voltage.Value>0).Select(x=>x.Voltage.Value).DefaultIfEmpty(0).Min(); }
        private static double ClipIncreaseWindow(LogAnalysisResult result, double start, double end)
        {
            List<VibeSample> v = result.Vibes.Where(x=>x.TimeMs>=start&&x.TimeMs<=end).ToList(); if(v.Count<2)return 0; double total=0;
            foreach(IGrouping<int,VibeSample> g in v.GroupBy(x=>x.Instance)) { List<VibeSample> o=g.OrderBy(x=>x.TimeMs).ToList(); if(o.Any(x=>x.Clip.HasValue)){double f=o.First(x=>x.Clip.HasValue).Clip.Value,l=o.Last(x=>x.Clip.HasValue).Clip.Value;total+=Math.Max(0,l-f);}else{VibeSample f=o[0],l=o[o.Count-1];total+=Math.Max(0,l.Clip0-f.Clip0)+Math.Max(0,l.Clip1-f.Clip1)+Math.Max(0,l.Clip2-f.Clip2);} } return total;
        }

        private static PrincipalIncident BaseIncident(LogAnalysisResult result, string category, string title, double start, double peak, double end, int line)
        {
            return new PrincipalIncident { FilePath=result.FilePath, FileName=Path.GetFileName(result.FilePath), Line=line, Id=category+"-"+Math.Round(start).ToString(CultureInfo.InvariantCulture), Category=category, Title=title, StartTimeMs=start, PeakTimeMs=peak, EndTimeMs=end, StartTime=FormatTime(result,start), PeakTime=FormatTime(result,peak), Phase=StateAt(result,start) };
        }

        private static void ApplyCoreTelemetryLimitations(LogAnalysisResult result, PrincipalIncident incident)
        {
            DataAvailabilityAssessment gps = result.DataAvailability.FirstOrDefault(a => a.Group == "GPS");
            if (gps == null || gps.Status != "Available")
                AddUnique(incident.UnavailableData, "GPS evidence is limited (" + (gps == null ? "status unknown" : gps.Status) + "), so position/speed-based interpretation remains conservative.");

            DataAvailabilityAssessment rc = result.DataAvailability.FirstOrDefault(a => a.Group == "RC / pilot inputs");
            if (rc == null || rc.Status != "Available")
                AddUnique(incident.UnavailableData, "RCIN/pilot-input evidence is limited (" + (rc == null ? "status unknown" : rc.Status) + "), so pilot-versus-autopilot attribution is uncertain.");

            if (!result.MavCommands.Any(c => c.TimeMs >= incident.StartTimeMs - 3000 && c.TimeMs <= incident.EndTimeMs + 3000))
                AddUnique(incident.UnavailableData, "No nearby MAVLink command record was logged around this incident window.");

            if (!result.Attitudes.Any(a => a.TimeMs >= incident.StartTimeMs - 1000 && a.TimeMs <= incident.EndTimeMs + 1000))
                AddUnique(incident.UnavailableData, "ATT data coverage is insufficient around this event window.");

            if (!result.Altitudes.Any(a => a.TimeMs >= incident.StartTimeMs - 1000 && a.TimeMs <= incident.EndTimeMs + 1000))
                AddUnique(incident.UnavailableData, "Altitude/climb evidence is insufficient around this event window.");

            if (result.SampleCaps.Any())
                AddUnique(incident.UnavailableData, "One or more telemetry streams hit analysis sample caps; verify the same window in raw logs for high-fidelity reconstruction.");
        }

        private static void AddUnique(List<string> list, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            if (!list.Contains(value)) list.Add(value);
        }

        private static void ScoreIncident(LogAnalysisResult result, PrincipalIncident incident, int supportStrength)
        {
            int support=incident.SupportingEvidence.Count+incident.Known.Count+supportStrength; int contradictions=incident.ContradictingEvidence.Count; int missing=incident.UnavailableData.Count;
            int available=result.DataAvailability.Count(a=>string.Equals(a.Status,"Available",StringComparison.OrdinalIgnoreCase)); int limited=result.DataAvailability.Count-available; int coverage=available>limited?6:(limited>available?-6:0);
            int eventScore=34+support*8-contradictions*7-missing*4+coverage; int causeScore=22+incident.StronglyInferred.Count*12+(incident.PossibleCauses.Count==1?10:0)-contradictions*8-missing*7+coverage/2;
            if(!string.IsNullOrEmpty(result.ParseError)){eventScore-=18;causeScore-=24;} if(result.Firmware==null||!result.Firmware.VersionKnown)causeScore-=7; if(result.Firmware!=null&&result.Firmware.CustomBuild)causeScore-=10;
            if (string.Equals(incident.CommandClassification, "Uncertain", StringComparison.OrdinalIgnoreCase)) { eventScore -= 6; causeScore -= 12; }
            if (result.SampleCaps.Count > 0) { eventScore -= 4; causeScore -= 8; }
            incident.EventConfidenceScore=Math.Max(0,Math.Min(99,eventScore)); incident.RootCauseConfidenceScore=Math.Max(0,Math.Min(95,causeScore)); incident.EventConfidence=ConfidenceLabel(incident.EventConfidenceScore); incident.RootCauseConfidence=ConfidenceLabel(incident.RootCauseConfidenceScore);
            int severity=15; bool airborne=FlightStateResolver.IsAirborneState(incident.Phase); if(airborne)severity+=20; if(ContainsAny(incident.Category,"Control","Impact","Landing"))severity+=12; if(incident.CommandClassification=="Uncommanded")severity+=15; if(incident.SupportingEvidence.Any(e=>e.IndexOf("saturation",StringComparison.OrdinalIgnoreCase)>=0))severity+=8; if(incident.SupportingEvidence.Any(e=>e.IndexOf("oscillation",StringComparison.OrdinalIgnoreCase)>=0))severity+=7;
            AltitudeSample alt=FindNearest(result.Altitudes,incident.PeakTimeMs,a=>a.TimeMs,1200); if(airborne&&alt!=null&&alt.Altitude.HasValue){if(alt.Altitude.Value<5)severity+=10;else if(alt.Altitude.Value<15)severity+=5;}
            double gyro=result.Imu.Where(i=>Math.Abs(i.TimeMs-incident.PeakTimeMs)<1200).Select(i=>Math.Max(Math.Abs(i.GyroXDegPerSec??0),Math.Max(Math.Abs(i.GyroYDegPerSec??0),Math.Abs(i.GyroZDegPerSec??0)))).DefaultIfEmpty(0).Max(); if(gyro>300)severity+=10;else if(gyro>180)severity+=5;
            if(incident.EndTimeMs>=result.LastTimeMs-1000)severity+=8; if(StateAt(result,result.LastTimeMs)=="Abnormal or uncertain ending")severity+=8;
            incident.SeverityScore=Math.Max(0,Math.Min(100,severity)); incident.Severity=severity>=70?"Critical":severity>=45?"Warning":"Advisory";
        }

        private static List<PrincipalIncident> DeduplicateIncidents(List<PrincipalIncident> incidents)
        {
            var kept=new List<PrincipalIncident>(); foreach(PrincipalIncident incident in incidents.OrderByDescending(i=>i.SeverityScore).ThenBy(i=>i.StartTimeMs))
            { PrincipalIncident existing=kept.FirstOrDefault(k=>string.Equals(k.Category,incident.Category,StringComparison.OrdinalIgnoreCase)&&RangesOverlap(k.StartTimeMs,k.EndTimeMs,incident.StartTimeMs,incident.EndTimeMs)); if(existing==null)kept.Add(incident); else { existing.StartTimeMs=Math.Min(existing.StartTimeMs,incident.StartTimeMs); existing.EndTimeMs=Math.Max(existing.EndTimeMs,incident.EndTimeMs); existing.SupportingEvidence.AddRange(incident.SupportingEvidence.Where(x=>!existing.SupportingEvidence.Contains(x))); existing.ContradictingEvidence.AddRange(incident.ContradictingEvidence.Where(x=>!existing.ContradictingEvidence.Contains(x))); if(incident.SeverityScore>existing.SeverityScore){existing.SeverityScore=incident.SeverityScore;existing.Severity=incident.Severity;} } } return kept;
        }
        private static bool RangesOverlap(double a1,double a2,double b1,double b2){return Math.Max(a1,b1)<=Math.Min(a2,b2)+1500;}

        private static void DeduplicateFindings(LogAnalysisResult result)
        {
            var kept=new List<Finding>(); foreach(Finding f in result.Findings.OrderByDescending(x=>x.SeverityRank).ThenBy(x=>x.TimeMs))
            { Finding ex=kept.FirstOrDefault(k=>string.Equals(k.Subsystem,f.Subsystem,StringComparison.OrdinalIgnoreCase)&&SimilarTitle(k.Title,f.Title)&&Math.Abs(k.TimeMs-f.TimeMs)<3000); if(ex==null)kept.Add(f); else { ex.OccurrenceCount+=Math.Max(1,f.OccurrenceCount); if(!string.IsNullOrEmpty(f.Evidence)&&!ex.Evidence.Contains(f.Evidence))ex.Evidence+=" Additional evidence: "+f.Evidence; } } result.Findings=kept;
        }
        private static void AssociateSupportingFindings(LogAnalysisResult result)
        { foreach(Finding f in result.Findings) { PrincipalIncident i=result.Incidents.FirstOrDefault(x=>f.TimeMs>=x.StartTimeMs-2000&&f.TimeMs<=x.EndTimeMs+2000&&IncidentRelated(x,f)); if(i!=null){f.SupportingIncidentId=i.Id;f.IsSupportingEvidence=true;} } }
        private static bool IncidentRelated(PrincipalIncident i,Finding f){string c=(i.Category??"").ToUpperInvariant(),s=(f.Subsystem??"").ToUpperInvariant(); if(c.Contains("CONTROL"))return ContainsAny(s,"ATTITUDE","MOTOR","VERTICAL","VIBRATION","CONTROL"); if(c.Contains("NAVIGATION")||c.Contains("ESTIMATOR"))return ContainsAny(s,"GPS","EKF","ESTIMATOR","COMPASS"); if(c.Contains("POWER")||c.Contains("BATTERY"))return ContainsAny(s,"BATTERY","POWER"); if(c.Contains("LANDING")||c.Contains("IMPACT"))return ContainsAny(s,"VIBRATION","VERTICAL","MOTOR","ATTITUDE","CONTROL","LOGGING"); if(c.Contains("RC/GCS"))return ContainsAny(s,"RC","AUTOPILOT EVENT","AUTOPILOT MESSAGE"); return false;}
        private static bool SimilarTitle(string a,string b){if(string.Equals(a,b,StringComparison.OrdinalIgnoreCase))return true;string aa=NormalizeTitle(a),bb=NormalizeTitle(b);return aa==bb||(aa.Length>12&&bb.Length>12&&(aa.Contains(bb)||bb.Contains(aa)));}
        private static string NormalizeTitle(string v){if(string.IsNullOrEmpty(v))return string.Empty;return new string(v.ToUpperInvariant().Where(c=>char.IsLetterOrDigit(c)||c==' ').ToArray()).Replace("  "," ").Trim();}
        private static string ConfidenceLabel(int score){return score>=80?"High":score>=55?"Medium":"Low";}
        private static string StateAt(LogAnalysisResult r,double t){return FlightStateResolver.StateAt(r.FlightStates,t);}
        private static string ModeAt(LogAnalysisResult r,double t){TimelineEvent m=r.Timeline.Where(x=>x.Type=="Mode"&&x.TimeMs<=t).OrderByDescending(x=>x.TimeMs).FirstOrDefault();return m==null?"Unknown":m.Text.Replace("Flight mode changed to ",string.Empty);}
        private static bool IsAutonomousMode(string m){return ContainsAny((m??"").ToUpperInvariant(),"AUTO","GUIDED","RTL","LAND","LOITER","CIRCLE","POS","BRAKE","QRTL","QLAND");}
        private static string FormatTime(LogAnalysisResult r,double t){double rel=Math.Max(0,t-r.FirstTimeMs);TimeSpan x=TimeSpan.FromMilliseconds(rel);return string.Format(CultureInfo.InvariantCulture,"{0:00}:{1:00}:{2:00}.{3:000}",(int)x.TotalHours,x.Minutes,x.Seconds,x.Milliseconds);}
        private static Finding NewFinding(LogAnalysisResult r,string sev,string conf,string sub,string title,string evidence,string rec,double t,int line,string phase){return new Finding{FilePath=r.FilePath,FileName=Path.GetFileName(r.FilePath),Severity=sev,Confidence=conf,Subsystem=sub,Title=title,Evidence=evidence,Recommendation=rec,TimeMs=t,Time=FormatTime(r,t),Line=line,Phase=phase,SeverityRank=sev=="Critical"?4:sev=="Warning"?3:sev=="Advisory"?2:1,OccurrenceCount=1};}
        private static T FindNearest<T>(IEnumerable<T> source,double time,Func<T,double> get,double max) where T:class {T best=null;double d=max+1;foreach(T x in source){double nd=Math.Abs(get(x)-time);if(nd<d){d=nd;best=x;}}return d<=max?best:null;}
        private static double PercentileCopy(IEnumerable<double> values,double p){List<double> v=values.Where(x=>!double.IsNaN(x)&&!double.IsInfinity(x)).OrderBy(x=>x).ToList();if(v.Count==0)return 0;double pos=(v.Count-1)*p;int lo=(int)Math.Floor(pos),hi=(int)Math.Ceiling(pos);return lo==hi?v[lo]:v[lo]+(v[hi]-v[lo])*(pos-lo);}
        private static double Pearson(List<double> x,List<double> y){int n=Math.Min(x.Count,y.Count);if(n<3)return 0;double mx=x.Take(n).Average(),my=y.Take(n).Average(),num=0,dx2=0,dy2=0;for(int i=0;i<n;i++){double dx=x[i]-mx,dy=y[i]-my;num+=dx*dy;dx2+=dx*dx;dy2+=dy*dy;}double den=Math.Sqrt(dx2*dy2);return den<=1e-9?0:num/den;}
        private static double Wrap180(double a){while(a>180)a-=360;while(a<-180)a+=360;return a;}
        private static bool ContainsAny(string text,params string[] values){if(string.IsNullOrEmpty(text))return false;return values.Any(v=>text.IndexOf(v,StringComparison.OrdinalIgnoreCase)>=0);}
    }

    internal static class ReportBuilder
    {
        public static string BuildNarrative(IEnumerable<LogAnalysisResult> results, bool hebrew)
        {
            string english = BuildNarrative(results);
            return hebrew ? Localization.Report(english) : english;
        }

        public static string BuildPlainText(IEnumerable<LogAnalysisResult> results, bool hebrew)
        {
            string english = BuildPlainText(results);
            return hebrew ? Localization.PlainText(english) : english;
        }

        public static string BuildHtml(IEnumerable<LogAnalysisResult> results, bool hebrew)
        {
            return hebrew ? BuildHtmlHebrew(results) : BuildHtml(results);
        }

        public static string BuildNarrative(IEnumerable<LogAnalysisResult> results)
        {
            var sb = new StringBuilder();
            sb.AppendLine("PLAIN-LANGUAGE FLIGHT STORY");
            sb.AppendLine("Created by Nadav Golan-Yanay");
            sb.AppendLine("© 2026 Nadav Golan-Yanay. All rights reserved.");
            sb.AppendLine("Generated: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            sb.AppendLine();

            foreach (LogAnalysisResult result in results)
            {
                sb.AppendLine(Path.GetFileName(result.FilePath));
                sb.AppendLine(new string('=', 80));
                sb.AppendLine(BuildNarrativeForResult(result));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static string BuildNarrativeForResult(LogAnalysisResult result)
        {
            var sb = new StringBuilder();
            PrincipalIncident principal = GetPrincipalIncident(result);
            FlightStatePoint finalState = GetFinalState(result);
            string outcome = ClassifyOutcome(result, principal, finalState);
            List<string> modes = GetModes(result);
            double armedSeconds = GetStateDurationSeconds(result, false);
            double airborneSeconds = GetStateDurationSeconds(result, true);
            AltitudeReportSummary altitudeSummary = GetAltitudeReportSummary(result);
            double? maxSpeed = GetMaximumGroundSpeed(result);
            double? maxDistance = GetMaximumDistance(result);

            sb.AppendLine("1. OVERALL VERDICT");
            sb.AppendLine("- " + outcome + ".");
            if (principal != null)
            {
                sb.AppendLine("- Principal incident: [" + principal.Severity + "] " + principal.Title + ".");
                sb.AppendLine("- Confidence the event occurred: " + principal.EventConfidence + ". Confidence in the proposed root cause: " + principal.RootCauseConfidence + ".");
            }
            else
            {
                sb.AppendLine("- No principal incident was identified from the available correlated evidence.");
            }
            if (!string.IsNullOrEmpty(result.ParseError))
                sb.AppendLine("- Log/parser limitation: " + result.ParseError);
            sb.AppendLine();

            sb.AppendLine("2. FLIGHT OUTCOME");
            sb.AppendLine("- Armed duration: " + FormatDuration(armedSeconds) + ".");
            sb.AppendLine("- Airborne duration: " + FormatDuration(airborneSeconds) + ".");
            sb.AppendLine("- Modes used: " + (modes.Count == 0 ? "not reliably available" : string.Join(" -> ", modes.ToArray())) + ".");
            sb.AppendLine("- Altitude range relative to the logged reference: " + FormatAltitudeRange(altitudeSummary) + ".");
            sb.AppendLine("- Maximum height above the armed-ground reference: " + FormatMetric(altitudeSummary == null ? (double?)null : altitudeSummary.MaxHeightAboveArmedGround, "m") + ".");
            sb.AppendLine("- Maximum ground speed (selected primary GPS): " + FormatMetric(maxSpeed, "m/s") + ".");
            sb.AppendLine("- Maximum distance from the first valid armed primary-GPS point: " + FormatMetric(maxDistance, "m") + ".");
            sb.AppendLine("- Final resolved state: " + (finalState == null ? "Unknown" : finalState.State) + ".");
            sb.AppendLine("- Log ending/integrity: " + (result.Integrity == null ? "Uncertain" : result.Integrity.Status) + " — " + (result.Integrity == null ? "not evaluated" : result.Integrity.Reason));
            sb.AppendLine("- Firmware interpretation: " + (result.Firmware == null ? "Unknown" : result.Firmware.Summary) + ".");
            sb.AppendLine();

            sb.AppendLine("3. PRINCIPAL INCIDENT");
            if (principal == null)
            {
                sb.AppendLine("- No composite incident reached the evidence threshold. Individual findings remain available in the Findings tab.");
            }
            else
            {
                sb.AppendLine("- What happened: " + SafeText(principal.WhatHappened, principal.Title));
                sb.AppendLine("- Start: " + principal.StartTime + "; peak: " + principal.PeakTime + "; phase: " + principal.Phase + ".");
                sb.AppendLine("- Peak severity: " + principal.Severity + " (composite score " + principal.SeverityScore.ToString(CultureInfo.InvariantCulture) + ").");
                sb.AppendLine("- Likely consequences: " + SafeText(principal.LikelyConsequences, "Not determined from the available evidence."));
                sb.AppendLine("- Command classification: " + principal.CommandClassification + ".");
                sb.AppendLine();

                AppendEvidenceGroup(sb, "Known from the log", principal.Known, "No additional direct facts were stored for this incident.");
                AppendEvidenceGroup(sb, "Strongly inferred", principal.StronglyInferred, "No strong inference beyond the direct evidence.");
                AppendEvidenceGroup(sb, "Possible causes", principal.PossibleCauses, "No specific root cause is supported strongly enough to rank above general possibilities.");
                AppendEvidenceGroup(sb, "Cannot be determined from available data", BuildIncidentLimitations(result, principal), "No major incident-specific data limitation was identified.");
            }
            sb.AppendLine();

            sb.AppendLine("4. EVIDENCE TIMELINE");
            List<string> evidenceTimeline = BuildEvidenceTimeline(result, principal);
            if (evidenceTimeline.Count == 0)
                sb.AppendLine("- No high-value event records were available for a concise timeline.");
            else
                foreach (string line in evidenceTimeline) sb.AppendLine("- " + line);
            sb.AppendLine();

            sb.AppendLine("5. WHAT WAS COMMANDED");
            foreach (string line in BuildCommandSummary(result, principal)) sb.AppendLine("- " + line);
            sb.AppendLine();

            sb.AppendLine("6. VEHICLE RESPONSE");
            foreach (string line in BuildVehicleResponseSummary(result, principal)) sb.AppendLine("- " + line);
            sb.AppendLine();

            sb.AppendLine("7. SYSTEMS THAT APPEARED HEALTHY / REDUCED-LIKELIHOOD CAUSES");
            List<string> healthy = BuildHealthyEvidence(result);
            if (healthy.Count == 0)
                sb.AppendLine("- The available data is not strong enough to positively reduce the likelihood of a specific subsystem cause.");
            else
                foreach (string line in healthy) sb.AppendLine("- " + line);
            sb.AppendLine("- These statements reduce likelihood only; they are not proof that a subsystem was fault-free.");
            sb.AppendLine();

            sb.AppendLine("8. POSSIBLE CAUSES RANKED BY EVIDENCE");
            List<string> causes = BuildRankedCauses(result, principal);
            if (causes.Count == 0) sb.AppendLine("- No root cause can be ranked reliably from this log alone.");
            else for (int i = 0; i < causes.Count; i++) sb.AppendLine((i + 1).ToString(CultureInfo.InvariantCulture) + ". " + causes[i]);
            sb.AppendLine();

            sb.AppendLine("9. MISSING OR LIMITING DATA");
            List<string> missing = BuildMissingDataSummary(result, principal);
            if (missing.Count == 0) sb.AppendLine("- No major data limitation was identified for the questions assessed by this report.");
            else foreach (string line in missing) sb.AppendLine("- " + line);
            sb.AppendLine();

            sb.AppendLine("10. RECOMMENDED NEXT DIAGNOSTIC STEPS");
            List<string> recommendations = BuildRecommendations(result, principal);
            if (recommendations.Count == 0)
                sb.AppendLine("- Compare this flight with a known-good flight using the same vehicle, firmware and configuration, and verify any remaining questions in raw Log Browse.");
            else
                for (int i = 0; i < recommendations.Count; i++) sb.AppendLine((i + 1).ToString(CultureInfo.InvariantCulture) + ". " + recommendations[i]);
            sb.AppendLine();

            sb.AppendLine("TECHNICAL CONTEXT");
            sb.AppendLine("- Parsed lines: " + result.TotalLines.ToString("N0", CultureInfo.InvariantCulture) + ".");
            sb.AppendLine("- Detailed findings: " + result.Findings.Count.ToString(CultureInfo.InvariantCulture) +
                          " (Critical " + result.Findings.Count(f => f.Severity == "Critical") +
                          ", Warning " + result.Findings.Count(f => f.Severity == "Warning") +
                          ", Advisory " + result.Findings.Count(f => f.Severity == "Advisory") + ").");
            sb.AppendLine("- Principal incidents: " + result.Incidents.Count.ToString(CultureInfo.InvariantCulture) +
                          " (Critical " + result.Incidents.Count(i => i.Severity == "Critical") +
                          ", Warning " + result.Incidents.Count(i => i.Severity == "Warning") +
                          ", Advisory " + result.Incidents.Count(i => i.Severity == "Advisory") + ").");
            sb.AppendLine("- Pilot-input samples: " + result.RcInputs.Count.ToString("N0", CultureInfo.InvariantCulture) + ". Executed MAVLink commands: " + result.MavCommands.Count.ToString("N0", CultureInfo.InvariantCulture) + ".");
            if (result.SampleCaps.Count > 0)
                sb.AppendLine("- Long-log sampling caps: " + string.Join("; ", result.SampleCaps.Take(4).Select(c => c.Stream + " dropped " + c.DroppedSamples.ToString("N0", CultureInfo.InvariantCulture) + " after cap " + c.Limit.ToString("N0", CultureInfo.InvariantCulture)).ToArray()) + ".");
            sb.AppendLine("- Detailed telemetry, secondary findings and raw evidence remain available in the Incidents, Findings, Timeline, Pilot & GCS, Graphs, Interactive Map, Values and Log data tabs.");
            sb.AppendLine();
            sb.AppendLine("LIMITATION: Automatic findings are evidence, not definitive proof. Root-cause conclusions should be checked against raw records, the physical vehicle, configuration history and a known-good comparison flight.");
            return sb.ToString();
        }

        private static PrincipalIncident GetPrincipalIncident(LogAnalysisResult result)
        {
            if (result == null || result.Incidents == null) return null;
            return result.Incidents.OrderByDescending(i => i.SeverityScore).ThenByDescending(i => i.EventConfidenceScore).FirstOrDefault();
        }

        private static FlightStatePoint GetFinalState(LogAnalysisResult result)
        {
            if (result == null || result.FlightStates == null || result.FlightStates.Count == 0) return null;
            return result.FlightStates[result.FlightStates.Count - 1];
        }

        private static double GetStateDurationSeconds(LogAnalysisResult result, bool airborneOnly)
        {
            if (result == null || result.FlightStates == null || result.FlightStates.Count == 0) return 0;
            double totalMs = 0;
            for (int i = 0; i < result.FlightStates.Count; i++)
            {
                FlightStatePoint state = result.FlightStates[i];
                double end = i + 1 < result.FlightStates.Count ? result.FlightStates[i + 1].TimeMs : result.LastTimeMs;
                if (end < state.TimeMs) continue;
                bool include = airborneOnly ? state.Airborne : state.Armed;
                if (include) totalMs += end - state.TimeMs;
            }
            return Math.Max(0, totalMs / 1000.0);
        }

        private static List<string> GetModes(LogAnalysisResult result)
        {
            var modes = new List<string>();
            if (result == null || result.Timeline == null) return modes;
            foreach (TimelineEvent item in result.Timeline.Where(t => t.Type == "Mode").OrderBy(t => t.TimeMs))
            {
                string text = item.Text ?? string.Empty;
                text = text.Replace("Flight mode changed to ", string.Empty).Trim();
                if (text.Length == 0) continue;
                if (modes.Count == 0 || !string.Equals(modes[modes.Count - 1], text, StringComparison.OrdinalIgnoreCase)) modes.Add(text);
            }
            return modes;
        }

        private sealed class AltitudeReportSummary
        {
            public double? MinimumLogged;
            public double? MaximumLogged;
            public double? ArmedGroundReference;
            public double? MaxHeightAboveArmedGround;
        }

        private static AltitudeReportSummary GetAltitudeReportSummary(LogAnalysisResult result)
        {
            if (result == null) return null;
            var summary = new AltitudeReportSummary();
            List<AltitudeSample> valid = result.Altitudes
                .Where(a => a.Altitude.HasValue && !double.IsNaN(a.Altitude.Value) && !double.IsInfinity(a.Altitude.Value))
                .OrderBy(a => a.TimeMs).ToList();

            if (valid.Count > 0)
            {
                summary.MinimumLogged = valid.Min(a => a.Altitude.Value);
                summary.MaximumLogged = valid.Max(a => a.Altitude.Value);

                double firstArmed = result.FlightStates.Where(s => s.Armed).Select(s => s.TimeMs).DefaultIfEmpty(result.FirstTimeMs).Min();
                double firstAirborne = result.FlightStates.Where(s => s.Airborne).Select(s => s.TimeMs).DefaultIfEmpty(double.MaxValue).Min();
                List<double> ground = valid.Where(a => a.TimeMs >= firstArmed && a.TimeMs < firstAirborne && a.TimeMs <= firstArmed + 5000)
                    .Select(a => a.Altitude.Value).ToList();
                if (ground.Count == 0)
                    ground = valid.Where(a => a.TimeMs >= firstArmed && a.TimeMs <= firstArmed + 1500).Select(a => a.Altitude.Value).ToList();
                if (ground.Count > 0)
                {
                    ground.Sort();
                    summary.ArmedGroundReference = ground[ground.Count / 2];
                    List<double> operational = valid.Where(a => a.TimeMs >= firstArmed).Select(a => a.Altitude.Value).ToList();
                    if (operational.Count > 0)
                        summary.MaxHeightAboveArmedGround = Math.Max(0, operational.Max() - summary.ArmedGroundReference.Value);
                }
                return summary;
            }

            List<GpsSample> gps = GetPrimaryGpsTrack(result, true);
            if (gps.Count > 0)
            {
                List<double> gpsAlt = gps.Where(g => g.Altitude.HasValue).Select(g => g.Altitude.Value).ToList();
                if (gpsAlt.Count > 0)
                {
                    summary.MinimumLogged = gpsAlt.Min();
                    summary.MaximumLogged = gpsAlt.Max();
                    double baseline = gpsAlt[0];
                    summary.ArmedGroundReference = baseline;
                    summary.MaxHeightAboveArmedGround = Math.Max(0, gpsAlt.Max() - baseline);
                    return summary;
                }
            }
            return summary;
        }

        private static string FormatAltitudeRange(AltitudeReportSummary summary)
        {
            if (summary == null || !summary.MinimumLogged.HasValue || !summary.MaximumLogged.HasValue) return "not available";
            return summary.MinimumLogged.Value.ToString("0.0", CultureInfo.InvariantCulture) + " to " +
                   summary.MaximumLogged.Value.ToString("0.0", CultureInfo.InvariantCulture) + " m";
        }

        private static double? GetMaximumGroundSpeed(LogAnalysisResult result)
        {
            List<GpsSample> gps = GetPrimaryGpsTrack(result, false);
            List<double> speeds = gps.Where(g => g.Speed.HasValue && !double.IsNaN(g.Speed.Value) && !double.IsInfinity(g.Speed.Value))
                .Select(g => Math.Abs(g.Speed.Value)).ToList();
            return speeds.Count == 0 ? (double?)null : speeds.Max();
        }

        private static double? GetMaximumDistance(LogAnalysisResult result)
        {
            if (result == null) return null;
            double firstArmed = result.FlightStates.Where(s => s.Armed).Select(s => s.TimeMs).DefaultIfEmpty(result.FirstTimeMs).Min();
            List<GpsSample> gps = GetPrimaryGpsTrack(result, true).Where(g => g.TimeMs >= firstArmed).OrderBy(g => g.TimeMs).ToList();
            if (gps.Count == 0) return null;

            List<GpsSample> accepted = FilterGpsMovementTrack(gps);
            if (accepted.Count == 0) return null;
            GpsSample origin = accepted[0];
            double max = 0;
            foreach (GpsSample sample in accepted)
            {
                double distance = HaversineMeters(origin.Lat.Value, origin.Lng.Value, sample.Lat.Value, sample.Lng.Value);
                if (distance > max) max = distance;
            }
            return max;
        }

        private static List<GpsSample> GetPrimaryGpsTrack(LogAnalysisResult result, bool requirePosition)
        {
            if (result == null || result.Gps == null) return new List<GpsSample>();
            var candidates = result.Gps
                .Where(p => SensorAssessmentBuilder.InferGpsOrdinal(p.Key) == 1)
                .Select(p => new
                {
                    Key = p.Key,
                    Samples = p.Value.Where(g => (!requirePosition || IsValidGpsPosition(g)) && (!g.Status.HasValue || g.Status.Value >= 3)).OrderBy(g => g.TimeMs).ToList()
                })
                .Where(p => p.Samples.Count > 0)
                .ToList();
            if (candidates.Count == 0) return new List<GpsSample>();

            bool anyExplicitlyUsed = candidates.Any(p => p.Samples.Any(g => g.Used.HasValue && g.Used.Value > 0.5));
            var selected = candidates
                .OrderByDescending(p => p.Samples.Count(g => g.Used.HasValue && g.Used.Value > 0.5))
                .ThenByDescending(p => p.Samples.Count)
                .First();
            List<GpsSample> samples = selected.Samples;
            if (anyExplicitlyUsed)
            {
                List<GpsSample> used = samples.Where(g => g.Used.HasValue && g.Used.Value > 0.5).ToList();
                if (used.Count > 0) samples = used;
            }
            return samples;
        }

        private static List<GpsSample> FilterGpsMovementTrack(List<GpsSample> samples)
        {
            var accepted = new List<GpsSample>();
            foreach (GpsSample sample in samples)
            {
                if (!IsValidGpsPosition(sample)) continue;
                if (accepted.Count == 0)
                {
                    accepted.Add(sample);
                    continue;
                }

                GpsSample previous = accepted[accepted.Count - 1];
                double dt = (sample.TimeMs - previous.TimeMs) / 1000.0;
                if (dt <= 0) continue;
                if (dt > 10)
                {
                    accepted.Add(sample);
                    continue;
                }
                double distance = HaversineMeters(previous.Lat.Value, previous.Lng.Value, sample.Lat.Value, sample.Lng.Value);
                double impliedSpeed = distance / dt;
                double loggedSpeed = Math.Max(Math.Abs(previous.Speed ?? 0), Math.Abs(sample.Speed ?? 0));
                double plausibleSpeed = Math.Max(120.0, loggedSpeed * 4.0 + 30.0);

                // Never let a single coordinate discontinuity dominate distance-to-origin metrics.
                // This threshold remains permissive for fast fixed-wing aircraft while rejecting
                // multi-kilometre receiver swaps, invalid startup coordinates and obvious jumps.
                if (distance > 100.0 && impliedSpeed > plausibleSpeed)
                    continue;
                accepted.Add(sample);
            }
            return accepted;
        }

        private static bool IsValidGpsPosition(GpsSample sample)
        {
            if (sample == null || !sample.Lat.HasValue || !sample.Lng.HasValue) return false;
            double lat = sample.Lat.Value, lng = sample.Lng.Value;
            if (double.IsNaN(lat) || double.IsInfinity(lat) || double.IsNaN(lng) || double.IsInfinity(lng)) return false;
            if (Math.Abs(lat) > 90 || Math.Abs(lng) > 180) return false;
            if (Math.Abs(lat) < 1e-8 && Math.Abs(lng) < 1e-8) return false;
            return true;
        }

        private static double HaversineMeters(double lat1, double lon1, double lat2, double lon2)
        {
            const double radius = 6371000.0;
            double dLat = (lat2 - lat1) * Math.PI / 180.0;
            double dLon = (lon2 - lon1) * Math.PI / 180.0;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(Math.Max(0, 1 - a)));
            return radius * c;
        }

        private static string ClassifyOutcome(LogAnalysisResult result, PrincipalIncident principal, FlightStatePoint finalState)
        {
            if (result == null) return "Outcome uncertain";
            if (!string.IsNullOrEmpty(result.ParseError)) return "Outcome uncertain because log integrity or parsing is incomplete";
            if (result.Integrity != null && string.Equals(result.Integrity.Status, "Incomplete", StringComparison.OrdinalIgnoreCase)) return "Outcome uncertain because the log file is physically incomplete";
            if (result.Integrity != null && string.Equals(result.Integrity.Status, "Abnormal", StringComparison.OrdinalIgnoreCase) && principal == null) return "Abnormal log ending without enough evidence to assign a root cause";
            if (finalState != null && string.Equals(finalState.State, "Abnormal or uncertain ending", StringComparison.OrdinalIgnoreCase))
                return principal == null ? "Uncertain ending" : "Probable incident with an abnormal or uncertain ending";
            if (principal != null && HasSparseKeyTelemetry(result))
                return "Flight outcome is uncertain from the available state evidence";
            if (principal != null && (string.Equals(principal.Severity, "Critical", StringComparison.OrdinalIgnoreCase) || string.Equals(principal.Severity, "Warning", StringComparison.OrdinalIgnoreCase)))
                return "Probable incident identified by correlated evidence";
            if (finalState != null && (string.Equals(finalState.State, "Disarmed", StringComparison.OrdinalIgnoreCase) || string.Equals(finalState.State, "Landed", StringComparison.OrdinalIgnoreCase)))
                return "Normal or apparently normal completed flight; no principal incident exceeded the correlation threshold";
            return "Flight outcome is uncertain from the available state evidence";
        }

        private static bool HasSparseKeyTelemetry(LogAnalysisResult result)
        {
            if (result == null) return true;
            // Conservative minimums for assigning a confident outcome narrative.
            const int attitudeMin = 8;
            const int altitudeMin = 8;
            const int rcMin = 6;
            const int gpsMin = 6;
            const int maxDurationScaledMin = 40;
            int scaled = Math.Min(maxDurationScaledMin, Math.Max(0, (int)Math.Ceiling(result.DurationSeconds * 0.2)));

            int missing = 0;
            if (result.Attitudes == null || result.Attitudes.Count < Math.Max(attitudeMin, scaled)) missing++;
            if (result.Altitudes == null || result.Altitudes.Count < Math.Max(altitudeMin, scaled)) missing++;
            bool rcSparse = result.RcInputs == null || result.RcInputs.Count < Math.Max(rcMin, scaled / 2);
            if (rcSparse) missing++;
            if (rcSparse && (result.MavCommands == null || result.MavCommands.Count == 0)) missing++;
            List<GpsSample> gps = GetPrimaryGpsTrack(result, true);
            if (gps.Count < Math.Max(gpsMin, scaled / 2)) missing++;
            return missing >= 2;
        }

        private static List<string> BuildEvidenceTimeline(LogAnalysisResult result, PrincipalIncident principal)
        {
            var lines = new List<string>();
            if (result == null) return lines;
            double start = principal == null ? result.FirstTimeMs : Math.Max(result.FirstTimeMs, principal.StartTimeMs - 10000);
            double end = principal == null ? result.LastTimeMs : Math.Min(result.LastTimeMs, principal.EndTimeMs + 5000);
            IEnumerable<TimelineEvent> timeline = result.Timeline.Where(t => t.TimeMs >= start && t.TimeMs <= end && (t.Type == "Arming" || t.Type == "Mode" || t.Type == "ERR" || t.Type == "EV" || t.Type == "MSG")).OrderBy(t => t.TimeMs);
            foreach (TimelineEvent item in timeline.Take(10))
                AddUnique(lines, item.Time + " | " + item.Type + " | " + item.Text);
            if (principal != null)
                foreach (string evidence in principal.SupportingEvidence.Take(6)) AddUnique(lines, principal.StartTime + " | correlated evidence | " + evidence);
            return lines.Take(14).ToList();
        }

        private static List<string> BuildCommandSummary(LogAnalysisResult result, PrincipalIncident principal)
        {
            var lines = new List<string>();
            if (principal != null) lines.Add("Classification: " + principal.CommandClassification + ". The tool does not assign blame unless command and response evidence agree.");
            double center = principal == null ? result.LastTimeMs : principal.StartTimeMs;
            List<MavCommandRecord> commands = result.MavCommands.Where(c => Math.Abs(c.TimeMs - center) <= 5000).OrderBy(c => c.TimeMs).Take(4).ToList();
            foreach (MavCommandRecord command in commands)
                lines.Add(command.Time + " — executed " + command.CommandName + " from " + command.SourceText + "; result " + command.ResultName + ".");
            if (commands.Count == 0)
                lines.Add("No executed MAVLink command was logged near the event; command-path attribution therefore remains conservative.");
            List<RcInputSample> rc = result.RcInputs.Where(r => Math.Abs(r.TimeMs - center) <= 2500).OrderBy(r => Math.Abs(r.TimeMs - center)).Take(3).OrderBy(r => r.TimeMs).ToList();
            if (rc.Count > 0)
            {
                foreach (RcInputSample sample in rc)
                    lines.Add(GetTimeTextForReport(result, sample.TimeMs) + " — pilot input roll " + FormatPercent(sample.Roll) + ", pitch " + FormatPercent(sample.Pitch) + ", throttle " + FormatPercent(sample.Throttle) + ", yaw " + FormatPercent(sample.Yaw) + (sample.PrimaryOverrideActive ? "; MAVLink RC override active." : "."));
            }
            else
                lines.Add("Pilot stick input is not available around the event; commanded-versus-uncommanded confidence is therefore limited.");
            return lines;
        }

        private static List<string> BuildVehicleResponseSummary(LogAnalysisResult result, PrincipalIncident principal)
        {
            var lines = new List<string>();
            if (principal != null)
            {
                foreach (string evidence in principal.SupportingEvidence.Where(e => e.IndexOf("desired", StringComparison.OrdinalIgnoreCase) >= 0 || e.IndexOf("rate", StringComparison.OrdinalIgnoreCase) >= 0 || e.IndexOf("saturation", StringComparison.OrdinalIgnoreCase) >= 0 || e.IndexOf("oscillation", StringComparison.OrdinalIgnoreCase) >= 0 || e.IndexOf("motor", StringComparison.OrdinalIgnoreCase) >= 0 || e.IndexOf("attitude", StringComparison.OrdinalIgnoreCase) >= 0).Take(6))
                    AddUnique(lines, evidence);
                foreach (string evidence in principal.ContradictingEvidence.Take(3)) AddUnique(lines, "Limiting evidence: " + evidence);
            }
            if (lines.Count == 0 && result.Attitudes.Count > 0)
                lines.Add("Attitude data is available, but no correlated control-response failure exceeded the incident threshold.");
            DataAvailabilityAssessment motorPerformance = result.DataAvailability.FirstOrDefault(a => a.Group == "Measured motor performance");
            if (motorPerformance == null || motorPerformance.Status != "Available")
                lines.Add("Commanded motor/servo output can be assessed from RCOU when present, but physical motor response cannot be verified without usable ESC/RPM telemetry.");
            return lines;
        }

        private static List<string> BuildHealthyEvidence(LogAnalysisResult result)
        {
            var lines = new List<string>();
            SensorAssessment gps = result.SensorAssessments.FirstOrDefault(s => s.Group == "GPS" && s.Instance == "1" && s.EnoughData);
            bool primaryGpsTrackConsistent = PrimaryGpsTrackIsConsistent(result);
            if (gps != null && primaryGpsTrackConsistent && !HasRelatedProblem(result, "GPS", "Navigation"))
                lines.Add("Primary GPS produced sufficient active, internally consistent data and no correlated GPS/navigation failure was identified; this evidence reduces the likelihood of primary-GPS loss as the initiating cause.");
            DataAvailabilityAssessment estimator = result.DataAvailability.FirstOrDefault(a => a.Group == "Estimator" && a.Status == "Available");
            if (estimator != null && !HasRelatedProblem(result, "EKF", "Estimator"))
                lines.Add("Estimator data was available without a correlated estimator-divergence incident; this evidence reduces the likelihood of an isolated EKF failure.");
            DataAvailabilityAssessment rc = result.DataAvailability.FirstOrDefault(a => a.Group == "RC / pilot inputs" && a.Status == "Available");
            if (rc != null && !HasRelatedProblem(result, "RC", "Radio", "failsafe"))
                lines.Add("Pilot-input data remained available and no correlated RC failsafe incident was identified; this evidence reduces the likelihood of RC-link loss.");
            if (result.MessageCounts.Keys.Any(k => k.StartsWith("POWR", StringComparison.OrdinalIgnoreCase)) && !HasRelatedProblem(result, "Power", "Voltage", "Vcc"))
                lines.Add("Flight-controller power records were present without a correlated controller-power event; this evidence reduces the likelihood of an obvious FC power-rail interruption.");
            SensorAssessment compass = result.SensorAssessments.FirstOrDefault(s => s.Group == "Compass" && s.EnoughData);
            if (compass != null && !HasRelatedProblem(result, "Compass", "Mag"))
                lines.Add("An active compass produced adequate data without a correlated compass fault; this evidence reduces the likelihood of a primary magnetic-sensor failure.");
            SensorAssessment baro = result.SensorAssessments.FirstOrDefault(s => s.Group == "Barometer" && s.EnoughData);
            if (baro != null && !HasRelatedProblem(result, "Baro", "Altitude"))
                lines.Add("Barometer data coverage was adequate without a correlated barometer fault; this evidence reduces the likelihood of a primary barometric-sensor failure.");
            if (string.IsNullOrEmpty(result.ParseError) && result.TotalLines > 0)
                lines.Add("The parser completed without a reported error; this supports basic log readability, although it does not by itself prove the file ended normally.");
            return lines;
        }

        private static bool PrimaryGpsTrackIsConsistent(LogAnalysisResult result)
        {
            List<GpsSample> raw = GetPrimaryGpsTrack(result, true);
            if (raw.Count < 2) return raw.Count > 0;
            List<GpsSample> filtered = FilterGpsMovementTrack(raw);
            if (filtered.Count < 2) return false;
            int rejected = raw.Count - filtered.Count;
            if (rejected > Math.Max(1, raw.Count / 20)) return false;
            return !result.Findings.Any(f => f.Subsystem == "GPS" &&
                (f.Title ?? string.Empty).IndexOf("position jump", StringComparison.OrdinalIgnoreCase) >= 0 &&
                (f.Severity == "Critical" || f.Severity == "Warning"));
        }

        private static bool HasRelatedProblem(LogAnalysisResult result, params string[] terms)
        {
            foreach (PrincipalIncident incident in result.Incidents)
            {
                string text = (incident.Category ?? string.Empty) + " " + (incident.Title ?? string.Empty) + " " + (incident.WhatHappened ?? string.Empty);
                if (terms.Any(term => text.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)) return true;
            }
            foreach (Finding finding in result.Findings.Where(f => f.Severity == "Critical" || f.Severity == "Warning"))
            {
                string text = (finding.Subsystem ?? string.Empty) + " " + (finding.Title ?? string.Empty) + " " + (finding.Evidence ?? string.Empty);
                if (terms.Any(term => text.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)) return true;
            }
            return false;
        }

        private static List<string> BuildRankedCauses(LogAnalysisResult result, PrincipalIncident principal)
        {
            var causes = new List<string>();
            if (principal != null)
            {
                foreach (string cause in principal.PossibleCauses) AddUnique(causes, cause + " [root-cause confidence: " + principal.RootCauseConfidence + "]");
                foreach (Finding finding in result.Findings.Where(f => !f.IsSupportingEvidence && (f.Severity == "Critical" || f.Severity == "Warning")).OrderByDescending(f => f.SeverityRank).ThenBy(f => f.TimeMs).Take(5))
                    AddUnique(causes, finding.Title + " — " + finding.Evidence);
            }
            return causes.Take(6).ToList();
        }

        private static List<string> BuildIncidentLimitations(LogAnalysisResult result, PrincipalIncident principal)
        {
            var limitations = new List<string>();
            if (principal != null)
                foreach (string item in principal.UnavailableData) AddUnique(limitations, item);

            if (principal != null)
            {
                string category = (principal.Category ?? string.Empty) + " " + (principal.Title ?? string.Empty);
                DataAvailabilityAssessment motor = result.DataAvailability.FirstOrDefault(a => a.Group == "Measured motor performance");
                if ((category.IndexOf("Control", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     category.IndexOf("Propulsion", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     category.IndexOf("Impact", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     category.IndexOf("Rollover", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     category.IndexOf("Landing", StringComparison.OrdinalIgnoreCase) >= 0) &&
                    (motor == null || motor.Status != "Available"))
                    AddUnique(limitations, "Physical motor/propeller response cannot be verified because usable ESC/RPM telemetry is unavailable; RCOU shows commanded output, not measured thrust or RPM.");

                if (principal.SupportingEvidence.Any(e => e.IndexOf("RATE data was unavailable", StringComparison.OrdinalIgnoreCase) >= 0 || e.IndexOf("RATE data", StringComparison.OrdinalIgnoreCase) >= 0 && e.IndexOf("insufficient", StringComparison.OrdinalIgnoreCase) >= 0))
                    AddUnique(limitations, "Detailed rate-loop control effectiveness cannot be fully reconstructed because RATE data was unavailable or insufficient near the event.");

                DataAvailabilityAssessment battery = result.DataAvailability.FirstOrDefault(a => a.Group == "Battery");
                if (battery != null && battery.Status != "Available")
                    AddUnique(limitations, "Battery voltage/current behavior cannot be used to confirm or exclude a battery-related initiating event because battery monitoring data is not available for this flight.");
            }

            if (!string.IsNullOrEmpty(result.ParseError))
                AddUnique(limitations, "The parser did not complete cleanly: " + result.ParseError);
            if (result.Integrity != null && result.Integrity.Status != "Normal")
                AddUnique(limitations, "The log ending is classified as " + result.Integrity.Status + "; the final sequence may be incomplete, which limits reconstruction of the event ending and recovery state.");

            return limitations.Take(10).ToList();
        }

        private static List<string> BuildMissingDataSummary(LogAnalysisResult result, PrincipalIncident principal)
        {
            var missing = new List<string>();
            if (principal != null)
                foreach (string item in principal.UnavailableData) AddUnique(missing, item);
            foreach (DataAvailabilityAssessment item in result.DataAvailability)
            {
                if (item.Status == "Available") continue;
                if (item.Status == "Monitoring disabled" || item.Status == "Sensor not installed" || item.Status == "Configured but unused")
                    AddUnique(missing, item.Group + ": " + item.Status + ". " + item.Reason);
                else if (item.Status == "Parser failure" || item.Status == "Data excluded from logging" || item.Status == "Message unsupported by firmware or not compiled" || item.Status == "Log too short to evaluate" || item.Status == "Startup-only / insufficient data" || item.Status == "No usable data despite declared format" || item.Status == "Reason uncertain")
                    AddUnique(missing, item.Group + ": " + item.Status + ". " + item.Reason);
            }
            if (!string.IsNullOrEmpty(result.ParseError)) AddUnique(missing, "Parser reported: " + result.ParseError);
            if (result.Integrity != null && result.Integrity.Status != "Normal") AddUnique(missing, "Log ending/integrity: " + result.Integrity.Status + ". " + result.Integrity.Reason);
            foreach (SampleCapAssessment cap in result.SampleCaps.Where(c => c.DroppedSamples > 0).OrderByDescending(c => c.DroppedSamples).Take(5))
                AddUnique(missing, "Sampling cap: " + cap.Stream + " kept up to " + cap.Limit.ToString("N0", CultureInfo.InvariantCulture) + " points and dropped " + cap.DroppedSamples.ToString("N0", CultureInfo.InvariantCulture) + " additional sample(s) (with sparse late replacements).");
            return missing.Take(10).ToList();
        }

        private static List<string> BuildRecommendations(LogAnalysisResult result, PrincipalIncident principal)
        {
            var recommendations = new List<string>();
            if (principal != null)
                foreach (string item in principal.Recommendations) AddUnique(recommendations, item);
            foreach (Finding finding in result.Findings.Where(f => f.Severity == "Critical" || f.Severity == "Warning").OrderByDescending(f => f.SeverityRank).ThenBy(f => f.TimeMs).Take(8))
                if (!string.IsNullOrWhiteSpace(finding.Recommendation)) AddUnique(recommendations, finding.Recommendation);
            foreach (DataAvailabilityAssessment item in result.DataAvailability)
            {
                if (item.Status == "Available" || item.Status == "Monitoring disabled" || item.Status == "Sensor not installed" || item.Status == "Configured but unused") continue;
                if (!string.IsNullOrWhiteSpace(item.Recommendation)) AddUnique(recommendations, item.Recommendation);
            }
            AddUnique(recommendations, "Compare the same signals with a known-good flight using the same vehicle, firmware and configuration before declaring a root cause.");
            return recommendations.Take(8).ToList();
        }

        private static void AppendEvidenceGroup(StringBuilder sb, string title, IEnumerable<string> values, string emptyText)
        {
            sb.AppendLine(title + ":");
            List<string> list = values == null ? new List<string>() : values.Where(v => !string.IsNullOrWhiteSpace(v)).Distinct(StringComparer.OrdinalIgnoreCase).Take(8).ToList();
            if (list.Count == 0) sb.AppendLine("- " + emptyText);
            else foreach (string value in list) sb.AppendLine("- " + value);
        }

        private static void AddUnique(List<string> list, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            if (!list.Any(x => string.Equals(x, value, StringComparison.OrdinalIgnoreCase))) list.Add(value);
        }

        private static string FormatDuration(double seconds)
        {
            if (seconds <= 0) return "00:00:00";
            TimeSpan span = TimeSpan.FromSeconds(seconds);
            return string.Format(CultureInfo.InvariantCulture, "{0:00}:{1:00}:{2:00}", (int)span.TotalHours, span.Minutes, span.Seconds);
        }

        private static string FormatMetric(double? value, string unit)
        {
            return value.HasValue ? value.Value.ToString("0.0", CultureInfo.InvariantCulture) + " " + unit : "not available";
        }

        private static string FormatPercent(double? value)
        {
            return value.HasValue ? value.Value.ToString("0", CultureInfo.InvariantCulture) + "%" : "n/a";
        }

        private static string SafeText(string value, string fallback)
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value;
        }

        public static string BuildPlainText(IEnumerable<LogAnalysisResult> results)
        {
            var sb = new StringBuilder();
            sb.AppendLine("COMPREHENSIVE LOG INVESTIGATION");
            sb.AppendLine("Created by Nadav Golan-Yanay");
            sb.AppendLine("© 2026 Nadav Golan-Yanay. All rights reserved.");
            sb.AppendLine("Generated: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            sb.AppendLine("This report is a diagnostic aid, not an airworthiness approval or proof of root cause.");
            sb.AppendLine(new string('=', 92));

            foreach (LogAnalysisResult result in results)
            {
                sb.AppendLine();
                sb.AppendLine(Path.GetFileName(result.FilePath));
                sb.AppendLine(new string('-', 92));
                sb.AppendLine("Path: " + result.FilePath);
                sb.AppendLine("Size: " + FormatBytes(new FileInfo(result.FilePath).Length));
                sb.AppendLine("Parsed lines: " + result.TotalLines.ToString("N0", CultureInfo.InvariantCulture));
                sb.AppendLine("Duration: " + TimeSpan.FromSeconds(result.DurationSeconds).ToString());
                sb.AppendLine("Message types: " + result.MessageCounts.Count.ToString(CultureInfo.InvariantCulture));
                sb.AppendLine("RC input samples: " + result.RcInputs.Count.ToString("N0", CultureInfo.InvariantCulture));
                sb.AppendLine("Executed MAVLink commands: " + result.MavCommands.Count.ToString("N0", CultureInfo.InvariantCulture));
                sb.AppendLine("Findings: " + result.Findings.Count.ToString(CultureInfo.InvariantCulture) +
                              " (Critical " + result.Findings.Count(f => f.Severity == "Critical") +
                              ", Warning " + result.Findings.Count(f => f.Severity == "Warning") +
                              ", Advisory " + result.Findings.Count(f => f.Severity == "Advisory") + ")");

                if (!string.IsNullOrEmpty(result.ParseError))
                    sb.AppendLine("Parser error: " + result.ParseError);

                sb.AppendLine();
                sb.AppendLine("Interpretation foundation:");
                sb.AppendLine("Firmware: " + (result.Firmware == null ? "Unknown" : result.Firmware.Summary));
                FlightStatePoint finalState = result.FlightStates == null || result.FlightStates.Count == 0 ? null : result.FlightStates[result.FlightStates.Count - 1];
                sb.AppendLine("Resolved final state: " + (finalState == null ? "Unknown" : finalState.State));
                sb.AppendLine("Log ending/integrity: " + (result.Integrity == null ? "Uncertain" : result.Integrity.Status) + " | " + (result.Integrity == null ? "not evaluated" : result.Integrity.Reason));
                if (result.FlightStates != null)
                    foreach (FlightStatePoint state in result.FlightStates.Take(20))
                        sb.AppendLine("  " + GetTimeTextForReport(result, state.TimeMs) + " | " + state.State + " | " + state.Evidence);

                sb.AppendLine("Sensor/configuration assessment:");
                foreach (SensorAssessment sensor in result.SensorAssessments)
                    sb.AppendLine("  " + sensor.Group + " " + sensor.Instance + " | " + sensor.Configuration + " | " + sensor.Activity + " | " + sensor.EstimatorSelection + " | samples " + sensor.SampleCount.ToString("N0", CultureInfo.InvariantCulture));

                sb.AppendLine("Data availability:");
                foreach (DataAvailabilityAssessment availability in result.DataAvailability)
                    sb.AppendLine("  " + availability.Group + " | " + availability.Status + " | " + availability.Reason);

                sb.AppendLine("Normalized signal groups: " + result.NormalizedSignals.Count.ToString(CultureInfo.InvariantCulture) + ". DataFlash engineering-unit scaling is trusted; magnitude-based unit guessing is not used.");
                if (result.SampleCaps.Count > 0)
                    sb.AppendLine("Long-log sampling caps: " + string.Join("; ", result.SampleCaps.Take(5).Select(c => c.Stream + " dropped " + c.DroppedSamples.ToString("N0", CultureInfo.InvariantCulture) + " after cap " + c.Limit.ToString("N0", CultureInfo.InvariantCulture)).ToArray()));

                sb.AppendLine();
                sb.AppendLine("Correlated principal incidents:");
                if (result.Incidents.Count == 0) sb.AppendLine("  None identified from the available correlated evidence.");
                foreach (PrincipalIncident incident in result.Incidents.Take(12))
                {
                    sb.AppendLine("  [" + incident.Severity + "] " + incident.StartTime + " | " + incident.Title + " | command=" + incident.CommandClassification + " | event confidence=" + incident.EventConfidence + " | root-cause confidence=" + incident.RootCauseConfidence);
                    foreach (string evidence in incident.SupportingEvidence.Take(4)) sb.AppendLine("    + " + evidence);
                    foreach (string evidence in incident.ContradictingEvidence.Take(2)) sb.AppendLine("    - " + evidence);
                }

                sb.AppendLine();
                sb.AppendLine("Highest-priority findings:");
                foreach (Finding finding in result.Findings.OrderByDescending(f => f.SeverityRank).ThenBy(f => f.TimeMs).Take(20))
                {
                    sb.AppendLine(string.Format(CultureInfo.InvariantCulture,
                        "[{0}] {1} | {2} | {3} | {4}", finding.Severity, finding.Time, finding.Subsystem, finding.Title, finding.Evidence));
                }

                sb.AppendLine();
                sb.AppendLine("Mode/events/messages:");
                foreach (TimelineEvent item in result.Timeline.Take(40))
                    sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0} | {1} | {2}", item.Time, item.Type, item.Text));
            }

            return sb.ToString();
        }

        private static string GetTimeTextForReport(LogAnalysisResult result, double timeMs)
        {
            double relative = result == null ? timeMs : timeMs - result.FirstTimeMs;
            if (relative <= 0 || double.IsNaN(relative) || double.IsInfinity(relative)) return "00:00:00.000";
            TimeSpan time = TimeSpan.FromMilliseconds(relative);
            return string.Format(CultureInfo.InvariantCulture, "{0:00}:{1:00}:{2:00}.{3:000}", (int)time.TotalHours, time.Minutes, time.Seconds, time.Milliseconds);
        }

        private static string BuildHtmlHebrew(IEnumerable<LogAnalysisResult> results)
        {
            List<LogAnalysisResult> list = results.ToList();
            var sb = new StringBuilder();
            sb.Append("<!doctype html><html lang='he' dir='rtl'><head><meta charset='utf-8'><title>חקירת לוג רחפן</title>");
            sb.Append("<style>body{font-family:Segoe UI,Arial,sans-serif;margin:28px;color:#222;direction:rtl;text-align:right}h1,h2{margin-bottom:6px}table{border-collapse:collapse;width:100%;margin:12px 0 28px;direction:rtl}th,td{border:1px solid #bbb;padding:7px;vertical-align:top;text-align:right}th{background:#eee}.Critical{background:#ffd7d7}.Warning{background:#fff2bd}.Advisory{background:#e7f2ff}.small{color:#555;font-size:90%}.mono{font-family:Consolas,monospace;direction:ltr;text-align:left;unicode-bidi:embed}pre{white-space:pre-wrap;direction:rtl;text-align:right}</style></head><body>");
            sb.Append("<h1>חקירת לוג רחפן מקיפה</h1><p><b>נוצר על ידי Nadav Golan-Yanay</b><br>© 2026 Nadav Golan-Yanay. כל הזכויות שמורות.</p>");
            sb.Append("<p class='small'>נוצר " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + ". כלי עזר אבחוני בלבד; יש לאמת כל ממצא ב-Mission Planner Log Browse.</p>");

            foreach (LogAnalysisResult result in list)
            {
                sb.Append("<h2>" + Html(Path.GetFileName(result.FilePath)) + "</h2>");
                sb.Append("<p><b>משך:</b> " + Html(TimeSpan.FromSeconds(result.DurationSeconds).ToString()) +
                          " &nbsp; <b>שורות:</b> " + result.TotalLines.ToString("N0", CultureInfo.InvariantCulture) +
                          " &nbsp; <b>ממצאים:</b> " + result.Findings.Count.ToString(CultureInfo.InvariantCulture) + "</p>");
                sb.Append("<pre>" + Html(Localization.Report(BuildNarrativeForResult(result))) + "</pre>");

                sb.Append("<h3>אירועים מרכזיים מתואמים</h3>");
                if (result.Incidents.Count == 0) sb.Append("<p>לא זוהה אירוע מרכזי מתוך הראיות המתואמות.</p>");
                else
                {
                    sb.Append("<table><thead><tr><th>חומרה</th><th>ביטחון באירוע</th><th>ביטחון בגורם</th><th>התחלה</th><th>שלב</th><th>אירוע</th><th>פקודה</th><th>ראיות</th></tr></thead><tbody>");
                    foreach (PrincipalIncident i in result.Incidents)
                        sb.Append("<tr class='" + Html(i.Severity) + "'><td>" + Html(Localization.Severity(i.Severity)) + "</td><td>" + Html(Localization.Confidence(i.EventConfidence)) + "</td><td>" + Html(Localization.Confidence(i.RootCauseConfidence)) + "</td><td class='mono'>" + Html(i.StartTime) + "</td><td>" + Html(Localization.Phase(i.Phase)) + "</td><td>" + Html(Localization.Analysis(i.Title)) + "</td><td>" + Html(Localization.Command(i.CommandClassification)) + "</td><td>" + Html(string.Join(" | ", i.SupportingEvidence.Take(5).Select(Localization.Analysis).ToArray())) + "</td></tr>");
                    sb.Append("</tbody></table>");
                }

                sb.Append("<h3>ממצאים מפורטים</h3><table><thead><tr><th>חומרה</th><th>ביטחון</th><th>זמן</th><th>שלב</th><th>תת-מערכת</th><th>ממצא</th><th>ראיה</th><th>המלצה</th><th>שורה</th></tr></thead><tbody>");
                foreach (Finding f in result.Findings.OrderByDescending(x => x.SeverityRank).ThenBy(x => x.TimeMs))
                {
                    sb.Append("<tr class='" + Html(f.Severity) + "'><td>" + Html(Localization.Severity(f.Severity)) + "</td><td>" + Html(Localization.Confidence(f.Confidence)) +
                              "</td><td class='mono'>" + Html(f.Time) + "</td><td>" + Html(Localization.Phase(f.Phase)) + "</td><td>" + Html(Localization.Analysis(f.Subsystem)) +
                              "</td><td>" + Html(Localization.Analysis(f.Title)) + "</td><td>" + Html(Localization.Analysis(f.Evidence)) + "</td><td>" + Html(Localization.Analysis(f.Recommendation)) +
                              "</td><td>" + f.Line.ToString(CultureInfo.InvariantCulture) + "</td></tr>");
                }
                sb.Append("</tbody></table>");

                sb.Append("<h3>נתוני טייס ופקודות GCS</h3>");
                sb.Append("<p><b>דגימות RCIN:</b> " + result.RcInputs.Count.ToString("N0", CultureInfo.InvariantCulture) +
                          " &nbsp; <b>פקודות MAVLink שבוצעו:</b> " + result.MavCommands.Count.ToString("N0", CultureInfo.InvariantCulture) +
                          ". ייתכן ש-MAVC אינו מכיל כל פעולה או הודעת טלמטריה שנשלחה מתחנת הקרקע.</p>");
                if (result.MavCommands.Count > 0)
                {
                    sb.Append("<table><thead><tr><th>זמן</th><th>שלב</th><th>מקור</th><th>יעד</th><th>פקודה</th><th>תוצאה</th><th>שיטה</th><th>פרמטרים</th><th>מיקום / XYZ</th></tr></thead><tbody>");
                    foreach (MavCommandRecord c in result.MavCommands.OrderBy(x => x.TimeMs))
                    {
                        sb.Append("<tr><td class='mono'>" + Html(c.Time) + "</td><td>" + Html(Localization.Phase(c.Phase)) + "</td><td>" + Html(c.SourceText) +
                                  "</td><td>" + Html(c.TargetText) + "</td><td>" + Html(c.CommandName + " (" + c.Command.ToString(CultureInfo.InvariantCulture) + ")") +
                                  "</td><td>" + Html(c.ResultName) + "</td><td>" + Html(c.TransportText) + "</td><td>" + Html(c.ParametersText) +
                                  "</td><td>" + Html(c.PositionText) + "</td></tr>");
                    }
                    sb.Append("</tbody></table>");
                }

                sb.Append("<h3>ציר זמן</h3><table><thead><tr><th>זמן</th><th>שלב</th><th>סוג</th><th>טקסט</th><th>שורה</th></tr></thead><tbody>");
                foreach (TimelineEvent t in result.Timeline)
                {
                    sb.Append("<tr><td class='mono'>" + Html(t.Time) + "</td><td>" + Html(Localization.Phase(t.Phase)) + "</td><td>" + Html(t.Type) +
                              "</td><td>" + Html(Localization.Analysis(t.Text)) + "</td><td>" + t.Line.ToString(CultureInfo.InvariantCulture) + "</td></tr>");
                }
                sb.Append("</tbody></table>");
            }

            sb.Append("<hr><p class='small'>נוצר על ידי Nadav Golan-Yanay — © 2026 Nadav Golan-Yanay. כל הזכויות שמורות.</p></body></html>");
            return sb.ToString();
        }

        public static string BuildHtml(IEnumerable<LogAnalysisResult> results)
        {
            List<LogAnalysisResult> list = results.ToList();
            var sb = new StringBuilder();
            sb.Append("<!doctype html><html><head><meta charset='utf-8'><title>Drone Log Investigation</title>");
            sb.Append("<style>body{font-family:Segoe UI,Arial,sans-serif;margin:28px;color:#222}h1,h2{margin-bottom:6px}table{border-collapse:collapse;width:100%;margin:12px 0 28px}th,td{border:1px solid #bbb;padding:7px;vertical-align:top;text-align:left}th{background:#eee}.Critical{background:#ffd7d7}.Warning{background:#fff2bd}.Advisory{background:#e7f2ff}.small{color:#555;font-size:90%}.mono{font-family:Consolas,monospace}</style></head><body>");
            sb.Append("<h1>Comprehensive Drone Log Investigation</h1>");
            sb.Append("<p><b>Created by Nadav Golan-Yanay</b><br>© 2026 Nadav Golan-Yanay. All rights reserved.</p>");
            sb.Append("<p class='small'>Generated " + Html(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)) + ". Diagnostic aid only; verify every finding in Mission Planner Log Browse.</p>");

            foreach (LogAnalysisResult result in list)
            {
                sb.Append("<h2>" + Html(Path.GetFileName(result.FilePath)) + "</h2>");
                sb.Append("<p><b>Duration:</b> " + Html(TimeSpan.FromSeconds(result.DurationSeconds).ToString()) +
                          " &nbsp; <b>Lines:</b> " + result.TotalLines.ToString("N0", CultureInfo.InvariantCulture) +
                          " &nbsp; <b>Findings:</b> " + result.Findings.Count.ToString(CultureInfo.InvariantCulture) + "</p>");
                sb.Append("<pre>" + Html(BuildNarrativeForResult(result)) + "</pre>");

                sb.Append("<h3>Correlated principal incidents</h3>");
                if (result.Incidents.Count == 0) sb.Append("<p>No principal incident was identified from correlated evidence.</p>");
                else
                {
                    sb.Append("<table><thead><tr><th>Severity</th><th>Event confidence</th><th>Cause confidence</th><th>Start</th><th>Phase</th><th>Incident</th><th>Command</th><th>Evidence</th></tr></thead><tbody>");
                    foreach (PrincipalIncident i in result.Incidents)
                        sb.Append("<tr class='" + Html(i.Severity) + "'><td>" + Html(i.Severity) + "</td><td>" + Html(i.EventConfidence) + "</td><td>" + Html(i.RootCauseConfidence) + "</td><td class='mono'>" + Html(i.StartTime) + "</td><td>" + Html(i.Phase) + "</td><td>" + Html(i.Title) + "</td><td>" + Html(i.CommandClassification) + "</td><td>" + Html(string.Join(" | ", i.SupportingEvidence.Take(5).ToArray())) + "</td></tr>");
                    sb.Append("</tbody></table>");
                }

                sb.Append("<h3>Detailed findings</h3><table><thead><tr><th>Severity</th><th>Confidence</th><th>Time</th><th>Phase</th><th>Subsystem</th><th>Finding</th><th>Evidence</th><th>Recommendation</th><th>Line</th></tr></thead><tbody>");
                foreach (Finding f in result.Findings.OrderByDescending(x => x.SeverityRank).ThenBy(x => x.TimeMs))
                {
                    sb.Append("<tr class='" + Html(f.Severity) + "'><td>" + Html(f.Severity) + "</td><td>" + Html(f.Confidence) +
                              "</td><td class='mono'>" + Html(f.Time) + "</td><td>" + Html(f.Phase) + "</td><td>" + Html(f.Subsystem) +
                              "</td><td>" + Html(f.Title) + "</td><td>" + Html(f.Evidence) + "</td><td>" + Html(f.Recommendation) +
                              "</td><td>" + f.Line.ToString(CultureInfo.InvariantCulture) + "</td></tr>");
                }
                sb.Append("</tbody></table>");

                sb.Append("<h3>Pilot and GCS command data</h3>");
                sb.Append("<p><b>RCIN samples:</b> " + result.RcInputs.Count.ToString("N0", CultureInfo.InvariantCulture) +
                          " &nbsp; <b>Executed MAVLink commands:</b> " + result.MavCommands.Count.ToString("N0", CultureInfo.InvariantCulture) +
                          ". MAVC may not contain every action or telemetry message sent by a ground station.</p>");
                if (result.MavCommands.Count > 0)
                {
                    sb.Append("<table><thead><tr><th>Time</th><th>Phase</th><th>Source</th><th>Target</th><th>Command</th><th>Result</th><th>Method</th><th>Parameters</th><th>Position / XYZ</th></tr></thead><tbody>");
                    foreach (MavCommandRecord c in result.MavCommands.OrderBy(x => x.TimeMs))
                    {
                        sb.Append("<tr><td class='mono'>" + Html(c.Time) + "</td><td>" + Html(c.Phase) + "</td><td>" + Html(c.SourceText) +
                                  "</td><td>" + Html(c.TargetText) + "</td><td>" + Html(c.CommandName + " (" + c.Command.ToString(CultureInfo.InvariantCulture) + ")") +
                                  "</td><td>" + Html(c.ResultName) + "</td><td>" + Html(c.TransportText) + "</td><td>" + Html(c.ParametersText) +
                                  "</td><td>" + Html(c.PositionText) + "</td></tr>");
                    }
                    sb.Append("</tbody></table>");
                }

                sb.Append("<h3>Timeline</h3><table><thead><tr><th>Time</th><th>Phase</th><th>Type</th><th>Text</th><th>Line</th></tr></thead><tbody>");
                foreach (TimelineEvent t in result.Timeline)
                {
                    sb.Append("<tr><td class='mono'>" + Html(t.Time) + "</td><td>" + Html(t.Phase) + "</td><td>" + Html(t.Type) +
                              "</td><td>" + Html(t.Text) + "</td><td>" + t.Line.ToString(CultureInfo.InvariantCulture) + "</td></tr>");
                }
                sb.Append("</tbody></table>");
            }

            sb.Append("<hr><p class='small'>Created by Nadav Golan-Yanay — © 2026 Nadav Golan-Yanay. All rights reserved.</p></body></html>");
            return sb.ToString();
        }

        private static string FormatBytes(long bytes)
        {
            string[] units = { "B", "KB", "MB", "GB" };
            double value = bytes;
            int unit = 0;
            while (value >= 1024 && unit < units.Length - 1)
            {
                value /= 1024;
                unit++;
            }
            return value.ToString("0.0", CultureInfo.InvariantCulture) + " " + units[unit];
        }

        private static string Html(string value)
        {
            if (value == null) return string.Empty;
            return value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")
                .Replace("\"", "&quot;").Replace("'", "&#39;");
        }
    }

    internal sealed class LogAnalysisResult
    {
        public string FilePath;
        public int TotalLines;
        public double FirstTimeMs;
        public double LastTimeMs;
        public double DurationSeconds;
        public string ParseError;
        public Dictionary<string, long> MessageCounts = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, List<string>> MessageFields = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, double> Parameters = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        public List<string> FirmwareMessages = new List<string>();
        public List<Finding> Findings = new List<Finding>();
        public List<TimelineEvent> Timeline = new List<TimelineEvent>();
        public Dictionary<string, List<BatterySample>> Batteries = new Dictionary<string, List<BatterySample>>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, List<GpsSample>> Gps = new Dictionary<string, List<GpsSample>>(StringComparer.OrdinalIgnoreCase);
        public List<VibeSample> Vibes = new List<VibeSample>();
        public List<AttitudeSample> Attitudes = new List<AttitudeSample>();
        public List<AltitudeSample> Altitudes = new List<AltitudeSample>();
        public List<MotorSample> Motors = new List<MotorSample>();
        public List<RcInputSample> RcInputs = new List<RcInputSample>();
        public List<MavCommandRecord> MavCommands = new List<MavCommandRecord>();
        public List<EkfSample> Ekf = new List<EkfSample>();
        public List<AhrsSample> Ahrs2 = new List<AhrsSample>();
        public List<ImuSample> Imu = new List<ImuSample>();
        public List<RateSample> Rates = new List<RateSample>();
        public List<MissionCommandRecord> MissionCommands = new List<MissionCommandRecord>();
        public List<ReferencePointRecord> ReferencePoints = new List<ReferencePointRecord>();
        public List<PrincipalIncident> Incidents = new List<PrincipalIncident>();
        public double EstimatorAgreementP95;
        public double EstimatorAgreementPeak;
        public double EkfCoreAgreementP95;
        public double EkfCoreAgreementPeak;
        public FirmwareProfile Firmware = new FirmwareProfile();
        public List<FlightStatePoint> FlightStates = new List<FlightStatePoint>();
        public List<SensorAssessment> SensorAssessments = new List<SensorAssessment>();
        public List<DataAvailabilityAssessment> DataAvailability = new List<DataAvailabilityAssessment>();
        public Dictionary<string, List<NormalizedSignalSample>> NormalizedSignals = new Dictionary<string, List<NormalizedSignalSample>>(StringComparer.OrdinalIgnoreCase);
        public LogIntegrityAssessment Integrity = new LogIntegrityAssessment();
        public List<SampleCapAssessment> SampleCaps = new List<SampleCapAssessment>();
        public int ItemParseErrors;
    }

    internal sealed class SampleCapAssessment
    {
        public string Stream;
        public int Limit;
        public long DroppedSamples;
        public long KeptTailReplacements;
    }

    internal sealed class Finding
    {
        public string FilePath;
        public string FileName;
        public string Severity;
        public string Confidence;
        public string Time;
        public double TimeMs;
        public string Phase;
        public string Subsystem;
        public string Title;
        public string Evidence;
        public string Recommendation;
        public int Line;
        public int SeverityRank;
        public int OccurrenceCount = 1;
        public bool IsSupportingEvidence;
        public string SupportingIncidentId;
    }

    internal sealed class TimelineEvent
    {
        public string FilePath;
        public string FileName;
        public string Time;
        public double TimeMs;
        public string Phase;
        public string Type;
        public string Text;
        public int Line;
    }

    internal sealed class FindingRow
    {
        public string FilePath { get; set; }
        public int LineNumber { get; set; }
        public string Severity { get; set; }
        public string Confidence { get; set; }
        public string File { get; set; }
        public string Time { get; set; }
        public double TimeMs { get; set; }
        public string Phase { get; set; }
        public string Subsystem { get; set; }
        public string Finding { get; set; }
        public string Evidence { get; set; }
        public string Recommendation { get; set; }
        public string Line { get; set; }

        public static FindingRow FromFinding(Finding f)
        {
            return new FindingRow
            {
                FilePath = f.FilePath,
                LineNumber = f.Line,
                Severity = f.Severity,
                Confidence = f.Confidence,
                File = f.FileName,
                Time = f.Time,
                TimeMs = f.TimeMs,
                Phase = f.Phase,
                Subsystem = f.Subsystem,
                Finding = f.Title,
                Evidence = f.Evidence,
                Recommendation = f.Recommendation,
                Line = f.Line > 0 ? f.Line.ToString(CultureInfo.InvariantCulture) : string.Empty
            };
        }
    }

    internal sealed class TimelineRow
    {
        public string FilePath { get; set; }
        public int LineNumber { get; set; }
        public string File { get; set; }
        public string Time { get; set; }
        public double TimeMs { get; set; }
        public string Phase { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public string Line { get; set; }

        public static TimelineRow FromEvent(TimelineEvent e)
        {
            return new TimelineRow
            {
                FilePath = e.FilePath,
                LineNumber = e.Line,
                File = e.FileName,
                Time = e.Time,
                TimeMs = e.TimeMs,
                Phase = e.Phase,
                Type = e.Type,
                Text = e.Text,
                Line = e.Line > 0 ? e.Line.ToString(CultureInfo.InvariantCulture) : string.Empty
            };
        }
    }

    internal sealed class MessageRow
    {
        public string File { get; set; }
        public string Message { get; set; }
        public string Count { get; set; }
        public string Fields { get; set; }
    }

    internal sealed class GcsCommandRow
    {
        public string FilePath { get; set; }
        public int LineNumber { get; set; }
        public double TimeMs { get; set; }
        public string Time { get; set; }
        public string Phase { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public string Command { get; set; }
        public string Result { get; set; }
        public string Method { get; set; }
        public string Parameters { get; set; }
        public string Position { get; set; }
        public string Line { get; set; }

        public static GcsCommandRow FromCommand(MavCommandRecord c)
        {
            return new GcsCommandRow
            {
                FilePath = c.FilePath,
                LineNumber = c.Line,
                TimeMs = c.TimeMs,
                Time = c.Time,
                Phase = c.Phase,
                Source = c.SourceText,
                Target = c.TargetText,
                Command = c.CommandName + " (" + c.Command.ToString(CultureInfo.InvariantCulture) + ")",
                Result = c.ResultName,
                Method = c.TransportText,
                Parameters = c.ParametersText,
                Position = c.PositionText,
                Line = c.Line > 0 ? c.Line.ToString(CultureInfo.InvariantCulture) : string.Empty
            };
        }
    }

    internal sealed class MissionCommandRecord
    {
        public double TimeMs;
        public int Total;
        public int Sequence;
        public int Command;
        public double? Param1;
        public double? Param2;
        public double? Param3;
        public double? Param4;
        public double? Lat;
        public double? Lng;
        public double? Altitude;
        public int Frame;
        public bool Executed;
        public int Line;
        public string Phase;
        public string CommandName { get { return MissionCommandNames.GetName(Command); } }
    }

    internal sealed class ReferencePointRecord
    {
        public double TimeMs;
        public int Type;
        public double Lat;
        public double Lng;
        public double? Altitude;
        public int Line;
        public string Phase;
        public string Label { get { return Type < 0 ? "Reference" : "ORGN type " + Type.ToString(CultureInfo.InvariantCulture); } }
    }

    internal sealed class ValueRow
    {
        public string Category { get; set; }
        public string Signal { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
        public string Source { get; set; }
        public string SampleTime { get; set; }
        public string Age { get; set; }
        public string Phase { get; set; }
        public string Notes { get; set; }

        public static ValueRow From(string category, string signal, double value, string unit, string source, double sampleTimeMs, double cursorTimeMs, string phase, string notes, LogAnalysisResult result, string format = "0.###")
        {
            return new ValueRow
            {
                Category = category, Signal = signal, Value = value.ToString(format, CultureInfo.InvariantCulture), Unit = unit ?? string.Empty,
                Source = source ?? string.Empty, SampleTime = Format(result, sampleTimeMs), Age = FormatAge(cursorTimeMs - sampleTimeMs),
                Phase = phase ?? "Unknown", Notes = notes ?? string.Empty
            };
        }

        public static ValueRow FromText(string category, string signal, string value, string source, double sampleTimeMs, double cursorTimeMs, string phase, string notes, LogAnalysisResult result)
        {
            return new ValueRow
            {
                Category = category, Signal = signal, Value = value ?? string.Empty, Unit = string.Empty, Source = source ?? string.Empty,
                SampleTime = Format(result, sampleTimeMs), Age = FormatAge(cursorTimeMs - sampleTimeMs), Phase = phase ?? "Unknown", Notes = notes ?? string.Empty
            };
        }

        private static string Format(LogAnalysisResult result, double timeMs)
        {
            double relative = result == null ? timeMs : Math.Max(0, timeMs - result.FirstTimeMs);
            TimeSpan t = TimeSpan.FromMilliseconds(relative);
            return string.Format(CultureInfo.InvariantCulture, "{0:00}:{1:00}:{2:00}.{3:000}", (int)t.TotalHours, t.Minutes, t.Seconds, t.Milliseconds);
        }

        private static string FormatAge(double ms)
        {
            double a = Math.Abs(ms);
            if (a < 1) return "0 ms";
            if (a < 1000) return a.ToString("0", CultureInfo.InvariantCulture) + " ms";
            return (a / 1000.0).ToString("0.00", CultureInfo.InvariantCulture) + " s";
        }
    }

    internal static class MissionCommandNames
    {
        public static string GetName(int command)
        {
            switch (command)
            {
                case 16: return "NAV_WAYPOINT";
                case 17: return "NAV_LOITER_UNLIM";
                case 18: return "NAV_LOITER_TURNS";
                case 19: return "NAV_LOITER_TIME";
                case 20: return "NAV_RETURN_TO_LAUNCH";
                case 21: return "NAV_LAND";
                case 22: return "NAV_TAKEOFF";
                case 82: return "NAV_SPLINE_WAYPOINT";
                case 84: return "NAV_VTOL_TAKEOFF";
                case 85: return "NAV_VTOL_LAND";
                default: return "MAV_CMD " + command.ToString(CultureInfo.InvariantCulture);
            }
        }
    }

    internal static class GpsDisplayHelper
    {
        public static string GetGpsTypeName(LogAnalysisResult result, int ordinal)
        {
            double value;
            string modern = "GPS" + ordinal.ToString(CultureInfo.InvariantCulture) + "_TYPE";
            string legacy = ordinal == 1 ? "GPS_TYPE" : "GPS_TYPE" + ordinal.ToString(CultureInfo.InvariantCulture);
            if (result == null || (!result.Parameters.TryGetValue(modern, out value) && !result.Parameters.TryGetValue(legacy, out value)))
                return "type not logged";
            int type = (int)Math.Round(value);
            switch (type)
            {
                case 0: return "Disabled"; case 1: return "Auto"; case 2: return "u-blox"; case 5: return "NMEA";
                case 6: return "SIRF"; case 7: return "HIL"; case 8: return "SwiftNav SBP"; case 9: return "UAVCAN/DroneCAN";
                case 10: return "Septentrio SBF"; case 11: return "GSOF"; case 13: return "ERB"; case 14: return "MAVLink GPS";
                case 15: return "NovaTel"; case 16: return "Hemisphere"; case 17: return "u-blox RTK base"; case 18: return "u-blox RTK rover";
                case 19: return "MSP"; case 20: return "AllyStar"; case 21: return "External AHRS"; case 22: return "DroneCAN RTK base";
                case 23: return "DroneCAN RTK rover"; case 24: return "Unicore NMEA"; case 25: return "Unicore moving baseline";
                case 26: return "SBF dual antenna"; case 100: return "SITL"; default: return "type " + type.ToString(CultureInfo.InvariantCulture);
            }
        }

        public static string SelectPrimaryKey(LogAnalysisResult result)
        {
            if (result == null || result.Gps == null || result.Gps.Count == 0) return null;
            var valid = result.Gps.Where(p => p.Value != null && p.Value.Any(IsValid)).ToList();
            if (valid.Count == 0) return null;
            var used = valid.Where(p => p.Value.Any(x => x.Used.HasValue && x.Used.Value > 0.5)).ToList();
            var source = used.Count > 0 ? used : valid;
            return source.OrderByDescending(p => p.Value.Count(x => x.Used.HasValue && x.Used.Value > 0.5))
                .ThenBy(p => SensorAssessmentBuilder.InferGpsOrdinal(p.Key))
                .ThenByDescending(p => p.Value.Count).Select(p => p.Key).FirstOrDefault();
        }

        private static bool IsValid(GpsSample s)
        {
            return s != null && s.Lat.HasValue && s.Lng.HasValue && Math.Abs(s.Lat.Value) <= 90 && Math.Abs(s.Lng.Value) <= 180 &&
                   !(Math.Abs(s.Lat.Value) < 0.000001 && Math.Abs(s.Lng.Value) < 0.000001);
        }
    }

    internal sealed class BatterySample
    {
        public double TimeMs;
        public double? Voltage;
        public double? Current;
        public double? UsedMah;
        public int Line;
        public string Phase;
    }

    internal sealed class VibeSample
    {
        public double TimeMs;
        public int Instance;
        public double? Clip;
        public double X;
        public double Y;
        public double Z;
        public double Clip0;
        public double Clip1;
        public double Clip2;
        public int Line;
        public string Phase;
    }

    internal sealed class GpsSample
    {
        public double TimeMs;
        public double? Status;
        public double? Sats;
        public double? Hdop;
        public double? Lat;
        public double? Lng;
        public double? Speed;
        public double? Course;
        public double? Altitude;
        public double? Used;
        public int Line;
        public string Phase;
        public bool Airborne;
    }

    internal sealed class AttitudeSample
    {
        public double TimeMs;
        public double? Roll;
        public double? DesiredRoll;
        public double? Pitch;
        public double? DesiredPitch;
        public double? Yaw;
        public double? DesiredYaw;
        public double? RollError;
        public double? PitchError;
        public double? YawError;
        public int Line;
        public string Phase;
        public bool Airborne;
    }

    internal sealed class AltitudeSample
    {
        public double TimeMs;
        public double? Altitude;
        public double? DesiredAltitude;
        public double? ClimbRate;
        public double? DesiredClimbRate;
        public string Mode;
        public int Line;
        public string Phase;
        public bool Airborne;
    }

    internal sealed class MotorSample
    {
        public double TimeMs;
        public Dictionary<int, double> Values;
        public int Line;
        public string Phase;
        public bool Airborne;
    }

    internal sealed class RcInputSample
    {
        public double TimeMs;
        public Dictionary<int, double> Values = new Dictionary<int, double>();
        public int OverrideMask;
        public int Flags;
        public int Line;
        public string Phase;
        public int RollChannel;
        public int PitchChannel;
        public int ThrottleChannel;
        public int YawChannel;
        public double? Roll;
        public double? Pitch;
        public double? Throttle;
        public double? Yaw;
        public bool PrimaryOverrideActive;

        public double? GetRaw(int channel)
        {
            double value;
            return Values != null && Values.TryGetValue(channel, out value) ? value : (double?)null;
        }
    }

    internal sealed class MavCommandRecord
    {
        public string FilePath;
        public double TimeMs;
        public string Time;
        public string Phase;
        public int TargetSystem;
        public int TargetComponent;
        public int SourceSystem;
        public int SourceComponent;
        public int Frame;
        public int Command;
        public string CommandName;
        public double? Param1;
        public double? Param2;
        public double? Param3;
        public double? Param4;
        public double? X;
        public double? Y;
        public double? Z;
        public int Result;
        public string ResultName;
        public bool WasCommandLong;
        public int Line;

        public string SourceText { get { return FormatId(SourceSystem, SourceComponent); } }
        public string TargetText { get { return FormatId(TargetSystem, TargetComponent); } }
        public string TransportText { get { return WasCommandLong ? "COMMAND_LONG" : "COMMAND_INT"; } }
        public string ParametersText
        {
            get
            {
                return "P1=" + Format(Param1) + "  P2=" + Format(Param2) + "  P3=" + Format(Param3) + "  P4=" + Format(Param4);
            }
        }
        public string PositionText
        {
            get
            {
                return "X=" + FormatCoordinate(X) + "  Y=" + FormatCoordinate(Y) + "  Z=" + Format(Z) + "  Frame=" + Frame.ToString(CultureInfo.InvariantCulture);
            }
        }

        private static string FormatId(int system, int component)
        {
            if (system < 0 && component < 0) return "n/a";
            return "sys " + system.ToString(CultureInfo.InvariantCulture) + " / comp " + component.ToString(CultureInfo.InvariantCulture);
        }
        private static string Format(double? value)
        {
            return value.HasValue ? value.Value.ToString("0.###", CultureInfo.InvariantCulture) : "n/a";
        }
        private static string FormatCoordinate(double? value)
        {
            if (!value.HasValue) return "n/a";
            double v = value.Value;
            if (Math.Abs(v) > 1000) v /= 10000000.0;
            return v.ToString("0.######", CultureInfo.InvariantCulture);
        }
    }

    internal static class MavCommandDecoder
    {
        private static readonly Dictionary<int, string> FallbackCommands = new Dictionary<int, string>
        {
            { 16, "MAV_CMD_NAV_WAYPOINT" }, { 17, "MAV_CMD_NAV_LOITER_UNLIM" },
            { 18, "MAV_CMD_NAV_LOITER_TURNS" }, { 19, "MAV_CMD_NAV_LOITER_TIME" },
            { 20, "MAV_CMD_NAV_RETURN_TO_LAUNCH" }, { 21, "MAV_CMD_NAV_LAND" },
            { 22, "MAV_CMD_NAV_TAKEOFF" }, { 176, "MAV_CMD_DO_SET_MODE" },
            { 177, "MAV_CMD_DO_JUMP" }, { 178, "MAV_CMD_DO_CHANGE_SPEED" },
            { 179, "MAV_CMD_DO_SET_HOME" }, { 183, "MAV_CMD_DO_SET_SERVO" },
            { 184, "MAV_CMD_DO_REPEAT_SERVO" }, { 193, "MAV_CMD_DO_PAUSE_CONTINUE" },
            { 300, "MAV_CMD_MISSION_START" }, { 400, "MAV_CMD_COMPONENT_ARM_DISARM" },
            { 401, "MAV_CMD_DO_FLIGHTTERMINATION" }, { 511, "MAV_CMD_REQUEST_MESSAGE" },
            { 520, "MAV_CMD_REQUEST_AUTOPILOT_CAPABILITIES" }
        };

        public static string CommandName(int command)
        {
            string reflected = ReflectEnumName("MAVLink.MAV_CMD", command);
            if (!string.IsNullOrEmpty(reflected)) return reflected;
            string fallback;
            return FallbackCommands.TryGetValue(command, out fallback) ? fallback : "MAV_CMD_" + command.ToString(CultureInfo.InvariantCulture);
        }

        public static string ResultName(int result)
        {
            string reflected = ReflectEnumName("MAVLink.MAV_RESULT", result);
            if (!string.IsNullOrEmpty(reflected)) return reflected.Replace("MAV_RESULT_", string.Empty);
            switch (result)
            {
                case 0: return "ACCEPTED";
                case 1: return "TEMPORARILY_REJECTED";
                case 2: return "DENIED";
                case 3: return "UNSUPPORTED";
                case 4: return "FAILED";
                case 5: return "IN_PROGRESS";
                case 6: return "CANCELLED";
                case 7: return "COMMAND_LONG_ONLY";
                case 8: return "COMMAND_INT_ONLY";
                case 9: return "UNSUPPORTED_FRAME";
                case 10: return "NOT_IN_CONTROL";
                default: return result < 0 ? "n/a" : "RESULT_" + result.ToString(CultureInfo.InvariantCulture);
            }
        }

        private static string ReflectEnumName(string fullName, int value)
        {
            try
            {
                foreach (System.Reflection.Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    Type type = assembly.GetType(fullName, false, true);
                    if (type == null || !type.IsEnum) continue;
                    string name = Enum.GetName(type, Enum.ToObject(type, value));
                    if (!string.IsNullOrEmpty(name)) return name;
                }
            }
            catch { }
            return string.Empty;
        }
    }

    internal sealed class EkfSample
    {
        public double TimeMs;
        public string Source;
        public int Core = -1;
        public double? Roll;
        public double? Pitch;
        public double? VelocityVariance;
        public double? PositionVariance;
        public double? HeightVariance;
        public double? MagneticVariance;
        public double? PositionInnovation;
        public double? VelocityInnovation;
        public int Line;
        public string Phase;
        public bool Airborne;
    }

    internal sealed class AhrsSample
    {
        public double TimeMs;
        public double? Roll;
        public double? Pitch;
        public double? Yaw;
        public int Line;
        public string Phase;
        public bool Airborne;
    }

    internal sealed class ImuSample
    {
        public double TimeMs;
        public string Source;
        public double? GyroXDegPerSec;
        public double? GyroYDegPerSec;
        public double? GyroZDegPerSec;
        public double? AccelX;
        public double? AccelY;
        public double? AccelZ;
        public int Line;
        public string Phase;
        public bool Airborne;
    }

    internal sealed class RateSample
    {
        public double TimeMs;
        public double? RollDesired;
        public double? RollActual;
        public double? PitchDesired;
        public double? PitchActual;
        public double? YawDesired;
        public double? YawActual;
        public int Line;
        public string Phase;
        public bool Airborne;
    }

    internal sealed class PrincipalIncident
    {
        public string FilePath;
        public string FileName;
        public int Line;
        public string Id;
        public string Category;
        public string Title;
        public double StartTimeMs;
        public double PeakTimeMs;
        public double EndTimeMs;
        public string StartTime;
        public string PeakTime;
        public string Phase;
        public string Severity;
        public int SeverityScore;
        public string EventConfidence;
        public int EventConfidenceScore;
        public string RootCauseConfidence;
        public int RootCauseConfidenceScore;
        public string CommandClassification = "Uncertain";
        public string WhatHappened;
        public string LikelyConsequences;
        public List<string> SupportingEvidence = new List<string>();
        public List<string> ContradictingEvidence = new List<string>();
        public List<string> Known = new List<string>();
        public List<string> StronglyInferred = new List<string>();
        public List<string> PossibleCauses = new List<string>();
        public List<string> UnavailableData = new List<string>();
        public List<string> Recommendations = new List<string>();
    }

    internal sealed class IncidentRow
    {
        public string FilePath { get; set; }
        public int LineNumber { get; set; }
        public double TimeMs { get; set; }
        public string Severity { get; set; }
        public string EventConfidence { get; set; }
        public string CauseConfidence { get; set; }
        public string File { get; set; }
        public string Time { get; set; }
        public string Phase { get; set; }
        public string Category { get; set; }
        public string Incident { get; set; }
        public string Command { get; set; }
        public string Evidence { get; set; }

        public static IncidentRow FromIncident(PrincipalIncident incident)
        {
            return new IncidentRow
            {
                FilePath = incident.FilePath,
                LineNumber = incident.Line,
                TimeMs = incident.StartTimeMs,
                Severity = incident.Severity,
                EventConfidence = incident.EventConfidence,
                CauseConfidence = incident.RootCauseConfidence,
                File = incident.FileName,
                Time = incident.StartTime,
                Phase = incident.Phase,
                Category = incident.Category,
                Incident = incident.Title,
                Command = incident.CommandClassification,
                Evidence = string.Join(" | ", incident.SupportingEvidence.Take(5).ToArray())
            };
        }
    }

    internal sealed class StateEvidence
    {
        public double TimeMs;
        public string Kind;
        public double Value;
        public string Text;
        public int Line;
    }

    internal sealed class FlightStatePoint
    {
        public double TimeMs;
        public string State;
        public bool Armed;
        public bool Airborne;
        public string Evidence;
    }

    internal sealed class FirmwareProfile
    {
        public string VehicleType = "Unknown";
        public string Version = "Unknown";
        public bool VersionKnown;
        public bool StandardArduPilot;
        public bool CustomBuild;
        public string Confidence = "Low";
        public HashSet<string> MessageFormats = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public string Summary
        {
            get
            {
                string build = CustomBuild ? "custom/modified build" : (StandardArduPilot ? "standard ArduPilot" : "unknown build");
                return VehicleType + " " + Version + " — " + build + ", interpretation confidence " + Confidence;
            }
        }
    }

    internal static class FirmwareProfileParser
    {
        public static void UpdateFromText(FirmwareProfile profile, string text)
        {
            if (profile == null || string.IsNullOrWhiteSpace(text)) return;
            string upper = text.ToUpperInvariant();
            if (upper.Contains("ARDUCOPTER") || upper.Contains("COPTER")) profile.VehicleType = "Copter";
            else if (upper.Contains("ARDUPLANE") || upper.Contains("PLANE")) profile.VehicleType = "Plane";
            else if (upper.Contains("ARDUROVER") || upper.Contains("ROVER")) profile.VehicleType = "Rover";
            else if (upper.Contains("ARDUSUB") || upper.Contains("SUB")) profile.VehicleType = "Sub";
            else if (upper.Contains("ANTENNATRACKER") || upper.Contains("TRACKER")) profile.VehicleType = "Tracker";

            int start = -1;
            for (int i = 0; i < text.Length - 2; i++)
            {
                if (char.IsDigit(text[i]) && text[i + 1] == '.' && char.IsDigit(text[i + 2])) { start = i; break; }
            }
            if (start >= 0)
            {
                int end = start;
                while (end < text.Length && (char.IsDigit(text[end]) || text[end] == '.' || text[end] == '-' || char.IsLetter(text[end]))) end++;
                string candidate = text.Substring(start, end - start).TrimEnd('.', '-');
                if (candidate.Count(c => c == '.') >= 1)
                {
                    profile.Version = candidate;
                    profile.VersionKnown = true;
                }
            }

            profile.CustomBuild = upper.Contains("CUSTOM") || upper.Contains("DIRTY") || upper.Contains("DEV") || upper.Contains("BETA") || upper.Contains("GIT:");
            profile.StandardArduPilot = profile.VehicleType != "Unknown" && !profile.CustomBuild;
            profile.Confidence = profile.StandardArduPilot && profile.VersionKnown ? "High" : (profile.VehicleType != "Unknown" ? "Medium" : "Low");
        }
    }

    internal static class FlightStateResolver
    {
        private sealed class Observation
        {
            public double TimeMs;
            public string Kind;
            public double A;
            public double B;
            public string Text;
        }

        public static List<FlightStatePoint> Resolve(LogAnalysisResult result, List<StateEvidence> evidence)
        {
            var observations = new List<Observation>();
            foreach (StateEvidence item in evidence)
                observations.Add(new Observation { TimeMs = item.TimeMs, Kind = item.Kind, A = item.Value, Text = item.Text });

            AddAltitudeObservations(observations, result.Altitudes);
            AddMotorObservations(observations, result.Motors);
            AddGpsObservations(observations, result.Gps);
            observations.Sort(delegate(Observation x, Observation y) { return x.TimeMs.CompareTo(y.TimeMs); });

            bool explicitArmKnown = false;
            bool armed = false;
            bool landed = true;
            bool airborneEver = false;
            bool motorActive = false;
            double altitude = 0;
            double climb = 0;
            double speed = 0;
            double takeoffStarted = -1;
            string lastState = string.Empty;
            var output = new List<FlightStatePoint>();

            if (observations.Count == 0)
            {
                output.Add(new FlightStatePoint { TimeMs = result.FirstTimeMs, State = "Disarmed", Armed = false, Airborne = false, Evidence = "No arming or motion evidence" });
                return output;
            }

            foreach (Observation observation in observations)
            {
                string kind = observation.Kind ?? string.Empty;
                if (kind == "ARM" || kind == "MAV_ARM") { armed = true; explicitArmKnown = true; }
                else if (kind == "DISARM" || kind == "MAV_DISARM") { armed = false; explicitArmKnown = true; landed = true; }
                else if (kind == "EVENT") ApplyEvent(result.Firmware, observation, ref armed, ref explicitArmKnown, ref landed);
                else if (kind == "ALT") { altitude = observation.A; climb = observation.B; }
                else if (kind == "MOTOR") { motorActive = observation.A > 0.5; if (!explicitArmKnown && motorActive) armed = true; }
                else if (kind == "GPS") speed = observation.A;

                bool motionAirborne = altitude > 1.5 || (altitude > 0.5 && climb > 0.8) || (speed > 4.0 && motorActive);
                bool airborne = armed && (!landed || motionAirborne);
                if (airborne) airborneEver = true;

                string state;
                bool previouslyAirborne = IsAirborneState(lastState);
                if (!armed) state = "Disarmed";
                else if (landed && !motionAirborne) state = airborneEver ? "Landed" : "Armed on ground";
                else if (airborne)
                {
                    if (!previouslyAirborne || takeoffStarted < 0) takeoffStarted = observation.TimeMs;
                    if ((observation.TimeMs - takeoffStarted) < 4000 || (altitude < 5 && climb > 0.6)) state = "Takeoff";
                    else if (altitude < 10 && climb < -0.6) state = "Landing";
                    else state = "Airborne";
                }
                else state = "Armed on ground";

                if (state == "Disarmed" || state == "Landed" || state == "Armed on ground")
                    takeoffStarted = -1;

                if (!string.Equals(state, lastState, StringComparison.Ordinal))
                {
                    output.Add(new FlightStatePoint
                    {
                        TimeMs = observation.TimeMs,
                        State = state,
                        Armed = armed,
                        Airborne = IsAirborneState(state),
                        Evidence = BuildEvidence(observation, altitude, climb, speed, motorActive)
                    });
                    lastState = state;
                }
            }

            if (output.Count == 0)
                output.Add(new FlightStatePoint { TimeMs = result.FirstTimeMs, State = armed ? "Armed on ground" : "Disarmed", Armed = armed, Airborne = false, Evidence = "Resolved from available evidence" });

            FlightStatePoint final = output[output.Count - 1];
            bool integrityIncomplete = result.Integrity != null && string.Equals(result.Integrity.Status, "Incomplete", StringComparison.OrdinalIgnoreCase);
            bool unresolvedArmedEnding = airborneEver && final.Armed && final.State != "Landed" && final.State != "Disarmed";
            if (!string.IsNullOrEmpty(result.ParseError) || integrityIncomplete || unresolvedArmedEnding)
            {
                output.Add(new FlightStatePoint
                {
                    TimeMs = result.LastTimeMs,
                    State = "Abnormal or uncertain ending",
                    Armed = final.Armed,
                    Airborne = final.Airborne,
                    Evidence = !string.IsNullOrEmpty(result.ParseError) ? "Parser did not reach a clean complete result" :
                        (integrityIncomplete ? "Physical file inspection indicates an incomplete final binary record or truncated tail" :
                        (!HasStrongEndingEvidence(observations, result.LastTimeMs) ? "Flight-state ending evidence is sparse or conflicting near the tail; normal landing/disarm cannot be proven" :
                        "Flight evidence exists but no normal landed/disarmed ending was resolved"))
                });
            }
            return output;
        }

        private static bool HasStrongEndingEvidence(List<Observation> observations, double lastTimeMs)
        {
            List<Observation> tail = observations.Where(o => o.TimeMs >= lastTimeMs - 6000).ToList();
            if (tail.Count < 5) return false;
            bool hasArmEvidence = tail.Any(o => o.Kind == "ARM" || o.Kind == "DISARM" || o.Kind == "MAV_ARM" || o.Kind == "MAV_DISARM" || o.Kind == "EVENT");
            bool hasKinematics = tail.Any(o => o.Kind == "ALT" || o.Kind == "GPS");
            bool hasMotor = tail.Any(o => o.Kind == "MOTOR");
            return (hasArmEvidence && hasKinematics) || (hasKinematics && hasMotor && tail.Count >= 8);
        }

        private static void ApplyEvent(FirmwareProfile firmware, Observation observation, ref bool armed, ref bool explicitArmKnown, ref bool landed)
        {
            if (firmware == null || !firmware.StandardArduPilot) return;
            int id = (int)Math.Round(observation.A);
            try
            {
                if (!Enum.IsDefined(typeof(DFLog.Log_Event), (byte)id)) return;
                string name = ((DFLog.Log_Event)(byte)id).ToString().ToUpperInvariant();
                if (name == "ARMED") { armed = true; explicitArmKnown = true; }
                else if (name == "DISARMED") { armed = false; explicitArmKnown = true; landed = true; }
                else if (name == "AUTO_ARMED") { armed = true; }
                else if (name == "LAND_COMPLETE" || name == "LAND_COMPLETE_MAYBE") landed = true;
                else if (name == "NOT_LANDED") landed = false;
            }
            catch { }
        }

        private static void AddAltitudeObservations(List<Observation> output, List<AltitudeSample> samples)
        {
            int stride = Math.Max(1, samples.Count / 50000);
            for (int i = 0; i < samples.Count; i += stride)
            {
                AltitudeSample s = samples[i];
                output.Add(new Observation { TimeMs = s.TimeMs, Kind = "ALT", A = s.Altitude ?? 0, B = s.ClimbRate ?? 0, Text = "Altitude/climb" });
            }
        }

        private static void AddMotorObservations(List<Observation> output, List<MotorSample> samples)
        {
            int stride = Math.Max(1, samples.Count / 50000);
            for (int i = 0; i < samples.Count; i += stride)
            {
                MotorSample s = samples[i];
                if (s.Values == null || s.Values.Count == 0) continue;
                double max = s.Values.Values.Max();
                double min = s.Values.Values.Min();
                bool pwm = max > 10;
                bool active = pwm ? max > 1150 : max > 0.08;
                output.Add(new Observation { TimeMs = s.TimeMs, Kind = "MOTOR", A = active ? 1 : 0, B = max - min, Text = "Motor/control output activity" });
            }
        }

        private static void AddGpsObservations(List<Observation> output, Dictionary<string, List<GpsSample>> gps)
        {
            foreach (KeyValuePair<string, List<GpsSample>> pair in gps)
            {
                if (SensorAssessmentBuilder.InferGpsOrdinal(pair.Key) != 1) continue;
                List<GpsSample> samples = pair.Value;
                int stride = Math.Max(1, samples.Count / 50000);
                for (int i = 0; i < samples.Count; i += stride)
                {
                    GpsSample s = samples[i];
                    if (!s.Speed.HasValue) continue;
                    output.Add(new Observation { TimeMs = s.TimeMs, Kind = "GPS", A = Math.Abs(s.Speed.Value), Text = "Primary GPS ground speed" });
                }
            }
        }

        private static string BuildEvidence(Observation observation, double altitude, double climb, double speed, bool motorActive)
        {
            return (observation.Text ?? observation.Kind) + "; alt=" + altitude.ToString("0.0", CultureInfo.InvariantCulture) +
                   " m, climb=" + climb.ToString("0.0", CultureInfo.InvariantCulture) + " m/s, speed=" + speed.ToString("0.0", CultureInfo.InvariantCulture) +
                   " m/s, motor activity=" + (motorActive ? "yes" : "no");
        }

        public static string StateAt(List<FlightStatePoint> states, double timeMs)
        {
            if (states == null || states.Count == 0) return "Unknown";
            FlightStatePoint selected = states[0];
            foreach (FlightStatePoint item in states)
            {
                if (item.TimeMs > timeMs) break;
                selected = item;
            }
            return selected.State;
        }

        public static bool AirborneAt(List<FlightStatePoint> states, double timeMs)
        {
            if (states == null || states.Count == 0) return false;
            FlightStatePoint selected = states[0];
            foreach (FlightStatePoint item in states)
            {
                if (item.TimeMs > timeMs) break;
                selected = item;
            }
            return selected.Airborne;
        }

        public static bool IsAirborneState(string state)
        {
            return string.Equals(state, "Takeoff", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(state, "Airborne", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(state, "Landing", StringComparison.OrdinalIgnoreCase);
        }
    }

    internal sealed class SensorAssessment
    {
        public string Group;
        public string Instance;
        public string Configuration;
        public string Activity;
        public string EstimatorSelection;
        public int SampleCount;
        public bool EnoughData;
        public string Conclusion;
    }

    internal static class SensorAssessmentBuilder
    {
        public static List<SensorAssessment> Build(LogAnalysisResult result)
        {
            var output = new List<SensorAssessment>();
            BuildGps(result, output);
            BuildBattery(result, output);
            BuildImu(result, output);
            BuildCompass(result, output);
            BuildBarometer(result, output);
            return output;
        }

        private static void BuildGps(LogAnalysisResult result, List<SensorAssessment> output)
        {
            for (int ordinal = 1; ordinal <= 3; ordinal++)
            {
                string modernParameter = "GPS" + ordinal.ToString(CultureInfo.InvariantCulture) + "_TYPE";
                string legacyParameter = ordinal == 1 ? "GPS_TYPE" : "GPS_TYPE" + ordinal.ToString(CultureInfo.InvariantCulture);
                double value;
                bool hasParam = TryGetParameter(result, new[] { modernParameter, legacyParameter }, out value);
                List<GpsSample> samples = result.Gps.Where(p => InferGpsOrdinal(p.Key) == ordinal).SelectMany(p => p.Value).ToList();
                bool anyUsed = samples.Any(s => s.Used.HasValue && s.Used.Value > 0.5);
                bool explicitUnused = samples.Count > 0 && samples.All(s => s.Used.HasValue && s.Used.Value < 0.5);
                bool disabled = hasParam && value <= 0;
                string configuration = disabled ? "Disabled / not installed" : (hasParam ? "Configured" : (samples.Count > 0 ? "Present; configuration not logged" : "Not present in log"));
                string activity = disabled ? "Inactive" : (explicitUnused ? "Configured but unused" : (samples.Count > 0 ? (anyUsed ? "Active / in use" : "Producing data") : "No data"));
                bool selected = EkfUsesGps(result) && !explicitUnused && !disabled;
                output.Add(new SensorAssessment
                {
                    Group = "GPS",
                    Instance = ordinal.ToString(CultureInfo.InvariantCulture),
                    Configuration = configuration,
                    Activity = activity,
                    EstimatorSelection = selected ? "GPS configured as an EKF source" : "Not proven selected by EKF",
                    SampleCount = samples.Count,
                    EnoughData = samples.Count >= 10 && !disabled && !explicitUnused,
                    Conclusion = disabled ? "Do not generate health warnings for this GPS." : (explicitUnused ? "Visible in the log but explicitly not in use; health warnings are suppressed." : "Evaluate only when flight-time data coverage is sufficient.")
                });
            }
        }

        private static void BuildBattery(LogAnalysisResult result, List<SensorAssessment> output)
        {
            for (int ordinal = 1; ordinal <= 3; ordinal++)
            {
                string parameter = ordinal == 1 ? "BATT_MONITOR" : "BATT" + ordinal.ToString(CultureInfo.InvariantCulture) + "_MONITOR";
                double value;
                bool hasParam = result.Parameters.TryGetValue(parameter, out value);
                int samples = result.Batteries.Where(p => InferBatteryOrdinal(p.Key) == ordinal).Sum(p => p.Value.Count);
                bool disabled = hasParam && value <= 0;
                output.Add(new SensorAssessment
                {
                    Group = "Battery",
                    Instance = ordinal.ToString(CultureInfo.InvariantCulture),
                    Configuration = disabled ? "Monitoring disabled" : (hasParam ? "Monitoring configured" : (samples > 0 ? "Present; configuration not logged" : "Not present in log")),
                    Activity = samples > 0 ? "Producing data" : "No data",
                    EstimatorSelection = "Not applicable",
                    SampleCount = samples,
                    EnoughData = samples >= 10 && !disabled,
                    Conclusion = disabled ? "Missing BAT data is expected; do not recommend a logging change as the first action." : "Battery conclusions require adequate voltage/current coverage."
                });
            }
        }

        private static void BuildImu(LogAnalysisResult result, List<SensorAssessment> output)
        {
            for (int ordinal = 1; ordinal <= 3; ordinal++)
            {
                string parameter = ordinal == 1 ? "INS_USE" : "INS_USE" + ordinal.ToString(CultureInfo.InvariantCulture);
                double value;
                bool hasParam = result.Parameters.TryGetValue(parameter, out value);
                string message = ordinal == 1 ? "IMU" : "IMU" + ordinal.ToString(CultureInfo.InvariantCulture);
                int samples = CountPrefix(result, message);
                bool disabled = hasParam && value <= 0;
                output.Add(new SensorAssessment
                {
                    Group = "IMU",
                    Instance = ordinal.ToString(CultureInfo.InvariantCulture),
                    Configuration = disabled ? "Disabled for estimation" : (hasParam ? "Enabled for estimation" : "Configuration unknown"),
                    Activity = samples > 0 ? "Producing data" : "No data",
                    EstimatorSelection = disabled ? "Not selected" : "Eligible for estimator use",
                    SampleCount = samples,
                    EnoughData = samples >= 10 && !disabled,
                    Conclusion = disabled ? "Do not report this secondary IMU as failed merely because flight-time data is absent." : "Health interpretation requires active flight-time samples."
                });
            }
        }

        private static void BuildCompass(LogAnalysisResult result, List<SensorAssessment> output)
        {
            for (int ordinal = 1; ordinal <= 3; ordinal++)
            {
                string parameter = ordinal == 1 ? "COMPASS_USE" : "COMPASS_USE" + ordinal.ToString(CultureInfo.InvariantCulture);
                double value;
                bool hasParam = result.Parameters.TryGetValue(parameter, out value);
                string message = ordinal == 1 ? "MAG" : "MAG" + ordinal.ToString(CultureInfo.InvariantCulture);
                int samples = CountPrefix(result, message);
                bool disabled = hasParam && value <= 0;
                output.Add(new SensorAssessment
                {
                    Group = "Compass",
                    Instance = ordinal.ToString(CultureInfo.InvariantCulture),
                    Configuration = disabled ? "Disabled" : (hasParam ? "Enabled" : "Configuration unknown"),
                    Activity = samples > 0 ? "Producing data" : "No data",
                    EstimatorSelection = EkfUsesCompass(result) && !disabled ? "Compass configured as an EKF yaw source" : "Not proven selected by EKF",
                    SampleCount = samples,
                    EnoughData = samples >= 10 && !disabled,
                    Conclusion = disabled ? "No health warning should be generated for this disabled compass." : "Evaluate only when selected/active and sufficiently logged."
                });
            }
        }

        private static void BuildBarometer(LogAnalysisResult result, List<SensorAssessment> output)
        {
            int samples = CountPrefix(result, "BARO");
            output.Add(new SensorAssessment
            {
                Group = "Barometer", Instance = "1+", Configuration = samples > 0 ? "Present" : "Unknown",
                Activity = samples > 0 ? "Producing data" : "No data", EstimatorSelection = EkfUsesBaro(result) ? "Barometer configured as an EKF height source" : "Not proven selected by EKF",
                SampleCount = samples, EnoughData = samples >= 10, Conclusion = "Absence alone is not a failure indication; firmware support and logging coverage are considered separately."
            });
        }

        public static bool ShouldEvaluateGps(LogAnalysisResult result, string key)
        {
            int ordinal = InferGpsOrdinal(key);
            SensorAssessment assessment = result.SensorAssessments.FirstOrDefault(a => a.Group == "GPS" && a.Instance == ordinal.ToString(CultureInfo.InvariantCulture));
            return assessment == null || assessment.EnoughData;
        }

        public static int InferGpsOrdinal(string key)
        {
            if (string.IsNullOrEmpty(key)) return 1;
            int open = key.IndexOf('[');
            int close = key.IndexOf(']');
            if (open >= 0 && close > open)
            {
                int instance;
                if (int.TryParse(key.Substring(open + 1, close - open - 1), NumberStyles.Integer, CultureInfo.InvariantCulture, out instance)) return instance + 1;
            }
            if (key.StartsWith("GPS3", StringComparison.OrdinalIgnoreCase)) return 3;
            if (key.StartsWith("GPS2", StringComparison.OrdinalIgnoreCase)) return 2;
            return 1;
        }

        private static int InferBatteryOrdinal(string key)
        {
            if (string.IsNullOrEmpty(key)) return 1;
            int open = key.IndexOf('[');
            int close = key.IndexOf(']');
            if (open >= 0 && close > open)
            {
                int instance;
                if (int.TryParse(key.Substring(open + 1, close - open - 1), NumberStyles.Integer, CultureInfo.InvariantCulture, out instance)) return instance + 1;
            }
            if (key.StartsWith("BAT3", StringComparison.OrdinalIgnoreCase)) return 3;
            if (key.StartsWith("BAT2", StringComparison.OrdinalIgnoreCase)) return 2;
            return 1;
        }

        private static bool TryGetParameter(LogAnalysisResult result, string[] names, out double value)
        {
            foreach (string name in names)
                if (result.Parameters.TryGetValue(name, out value)) return true;
            value = 0;
            return false;
        }

        private static int CountPrefix(LogAnalysisResult result, string prefix)
        {
            long count = result.MessageCounts.Where(p => p.Key.Equals(prefix, StringComparison.OrdinalIgnoreCase) || p.Key.StartsWith(prefix + "[", StringComparison.OrdinalIgnoreCase)).Sum(p => p.Value);
            return (int)Math.Min(int.MaxValue, count);
        }

        private static bool EkfUsesGps(LogAnalysisResult result) { return EkfSourceValuePresent(result, 3); }
        private static bool EkfUsesBaro(LogAnalysisResult result) { return result.Parameters.Any(p => p.Key.StartsWith("EK3_SRC", StringComparison.OrdinalIgnoreCase) && p.Key.EndsWith("POSZ", StringComparison.OrdinalIgnoreCase) && Math.Abs(p.Value - 1) < 0.1); }
        private static bool EkfUsesCompass(LogAnalysisResult result) { return result.Parameters.Any(p => p.Key.StartsWith("EK3_SRC", StringComparison.OrdinalIgnoreCase) && p.Key.EndsWith("YAW", StringComparison.OrdinalIgnoreCase) && Math.Abs(p.Value - 1) < 0.1); }
        private static bool EkfSourceValuePresent(LogAnalysisResult result, double value) { return result.Parameters.Any(p => p.Key.StartsWith("EK3_SRC", StringComparison.OrdinalIgnoreCase) && Math.Abs(p.Value - value) < 0.1); }
    }

    internal sealed class DataAvailabilityAssessment
    {
        public string Group;
        public string Status;
        public string Reason;
        public string Recommendation;
        public string Confidence;
        public int SampleCount;
    }

    internal static class DataAvailabilityBuilder
    {
        public static List<DataAvailabilityAssessment> Build(LogAnalysisResult result)
        {
            var output = new List<DataAvailabilityAssessment>();
            Add(output, result, "Attitude", new[] { "ATT" }, null);
            Add(output, result, "Control outputs", new[] { "RCOU", "RCO2", "RCO3" }, null);
            Add(output, result, "RC / pilot inputs", new[] { "RCIN", "RCI2" }, null);
            Add(output, result, "GPS", new[] { "GPS", "GPS2", "GPS3" }, DetermineDisabledGroup(result, "GPS"));
            Add(output, result, "Battery", new[] { "BAT", "BAT2", "BAT3", "CURR" }, DetermineDisabledGroup(result, "Battery"));
            Add(output, result, "Compass", new[] { "MAG", "MAG2", "MAG3" }, DetermineDisabledGroup(result, "Compass"));
            Add(output, result, "Barometer", new[] { "BARO", "BAR2", "BAR3" }, null);
            Add(output, result, "Vibration", new[] { "VIBE" }, null);
            Add(output, result, "Estimator", new[] { "XKF", "NKF", "AHR2" }, null);
            Add(output, result, "Measured motor performance", new[] { "ESC", "RPM" }, null, true);
            return output;
        }

        private static string DetermineDisabledGroup(LogAnalysisResult result, string group)
        {
            List<SensorAssessment> sensors = result.SensorAssessments.Where(s => s.Group == group).ToList();
            if (sensors.Count == 0) return null;
            bool anyExplicitDisabled = sensors.Any(s => s.Configuration.IndexOf("Disabled", StringComparison.OrdinalIgnoreCase) >= 0 || s.Configuration.IndexOf("not installed", StringComparison.OrdinalIgnoreCase) >= 0 || s.Configuration.IndexOf("Monitoring disabled", StringComparison.OrdinalIgnoreCase) >= 0);
            bool allDisabledOrAbsent = sensors.All(s => s.Configuration.IndexOf("Disabled", StringComparison.OrdinalIgnoreCase) >= 0 || s.Configuration.IndexOf("not installed", StringComparison.OrdinalIgnoreCase) >= 0 || s.Configuration.IndexOf("Monitoring disabled", StringComparison.OrdinalIgnoreCase) >= 0 || s.Configuration == "Not present in log");
            if (anyExplicitDisabled && allDisabledOrAbsent)
                return group == "Battery" ? "Monitoring disabled" : "Sensor not installed";
            if (sensors.Any(s => s.Activity == "Configured but unused") && !sensors.Any(s => s.EnoughData)) return "Configured but unused";
            return null;
        }

        private static void Add(List<DataAvailabilityAssessment> output, LogAnalysisResult result, string group, string[] prefixes, string disabledStatus, bool optional = false)
        {
            int count = (int)Math.Min(int.MaxValue, result.MessageCounts.Where(p => prefixes.Any(prefix => p.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))).Sum(p => p.Value));
            bool formatDeclared = result.MessageFields.Keys.Any(k => prefixes.Any(prefix => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)));
            var item = new DataAvailabilityAssessment { Group = group, SampleCount = count, Confidence = "High" };

            if (!string.IsNullOrEmpty(result.ParseError))
            {
                item.Status = "Parser failure";
                item.Reason = "The parser reported an error, so absence of later records cannot be interpreted reliably.";
                item.Recommendation = "Verify file integrity and open the raw log in Mission Planner before changing configuration or hardware.";
            }
            else if (!string.IsNullOrEmpty(disabledStatus))
            {
                item.Status = disabledStatus;
                item.Reason = "Logged parameters/usage indicate this function is not active for flight-health monitoring.";
                item.Recommendation = "Do not treat the missing data as a sensor failure. Change configuration only if this function is intended to be used.";
            }
            else if (count >= 10)
            {
                item.Status = "Available";
                item.Reason = count.ToString("N0", CultureInfo.InvariantCulture) + " records are available.";
                item.Recommendation = "No logging change is required for this data group.";
            }
            else if (result.DurationSeconds < 3.0 || !result.FlightStates.Any(s => s.Armed))
            {
                item.Status = "Log too short to evaluate";
                item.Reason = "There is not enough armed/operational time to decide whether this data should have appeared.";
                item.Recommendation = "Use a complete operational flight log before diagnosing missing " + group.ToLowerInvariant() + " data.";
            }
            else if (count > 0)
            {
                item.Status = "Startup-only / insufficient data";
                item.Reason = "Only " + count.ToString(CultureInfo.InvariantCulture) + " records were found.";
                item.Recommendation = "Check whether the subsystem became inactive after startup before changing logging settings.";
            }
            else if (result.Parameters.ContainsKey("LOG_BITMASK") && Math.Abs(result.Parameters["LOG_BITMASK"]) < 0.0001)
            {
                item.Status = "Data excluded from logging";
                item.Reason = "LOG_BITMASK is zero and no compatible records were present.";
                item.Recommendation = "Enable only the logging categories needed for the next diagnostic flight after confirming the subsystem itself is configured correctly.";
            }
            else if (!formatDeclared && result.Firmware != null && result.Firmware.VersionKnown && !result.Firmware.CustomBuild)
            {
                item.Status = "Message unsupported by firmware or not compiled";
                item.Reason = "The log did not declare a compatible message format for this data group.";
                item.Recommendation = optional ? "This is a limitation, not a fault. Add the telemetry only if physical-response verification is needed." : "Confirm this firmware/vehicle supports the expected message before changing logging parameters.";
                item.Confidence = "Medium";
            }
            else if (formatDeclared)
            {
                item.Status = "No usable data despite declared format";
                item.Reason = "The message format exists in the log, but no usable flight records were found. The source may be inactive, unused, or excluded by the runtime logging profile.";
                item.Recommendation = "Check subsystem activity/selection first; only then review the logging profile.";
                item.Confidence = "Medium";
            }
            else
            {
                item.Status = "Reason uncertain";
                item.Reason = "The available log cannot distinguish unsupported firmware, logging exclusion, or an inactive source.";
                item.Recommendation = "Confirm firmware capability and subsystem configuration before requesting more logging.";
                item.Confidence = "Low";
            }
            output.Add(item);
        }
    }

    internal sealed class LogIntegrityAssessment
    {
        public string Status = "Uncertain";
        public string Reason = "Not evaluated";
        public string Confidence = "Low";
        public bool BinaryLog;
        public bool TailComplete;
        public bool NoDisarmEvent;
        public bool ArmedAtEnd;
        public bool PowerRailEvidence;
        public long FileBytes;
        public long CompleteRecords;
        public long UnexpectedBytes;
        public long UnknownPacketTypes;
    }

    internal static class LogIntegrityBuilder
    {
        public static LogIntegrityAssessment InspectPhysicalFile(string filePath, Dictionary<int, int> messageLengths, int parserRecordCount)
        {
            var assessment = new LogIntegrityAssessment();
            var info = new FileInfo(filePath);
            assessment.FileBytes = info.Exists ? info.Length : 0;
            if (!info.Exists || info.Length == 0)
            {
                assessment.Status = "Incomplete";
                assessment.Reason = "The file is empty or unavailable.";
                assessment.Confidence = "High";
                return assessment;
            }

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                int first = stream.ReadByte();
                int second = stream.ReadByte();
                assessment.BinaryLog = first == 0xA3 && second == 0x95;
                stream.Position = 0;
                if (!assessment.BinaryLog)
                {
                    assessment.Status = "Uncertain";
                    assessment.Reason = "Text/non-binary log: physical DataFlash packet-tail validation is not applicable; logical ending is evaluated separately.";
                    assessment.Confidence = "Medium";
                    assessment.TailComplete = true;
                    return assessment;
                }

                long position = 0;
                long length = stream.Length;
                bool incompleteTail = false;
                while (position < length)
                {
                    stream.Position = position;
                    int b1 = stream.ReadByte();
                    if (b1 < 0) break;
                    int b2 = stream.ReadByte();
                    int type = stream.ReadByte();
                    if (b2 < 0 || type < 0)
                    {
                        incompleteTail = true;
                        break;
                    }
                    if (b1 != 0xA3 || b2 != 0x95)
                    {
                        assessment.UnexpectedBytes++;
                        position++;
                        continue;
                    }

                    int packetLength;
                    if (!messageLengths.TryGetValue(type, out packetLength) || packetLength < 3)
                    {
                        // FMT itself has a fixed DataFlash packet length of 89 bytes.
                        if (type == 0x80) packetLength = 89;
                        else
                        {
                            assessment.UnknownPacketTypes++;
                            position++;
                            continue;
                        }
                    }

                    if (position + packetLength > length)
                    {
                        incompleteTail = true;
                        break;
                    }
                    assessment.CompleteRecords++;
                    position += packetLength;
                }

                assessment.TailComplete = !incompleteTail && position == length;
                if (incompleteTail)
                {
                    assessment.Status = "Incomplete";
                    assessment.Reason = "The binary file ends before the declared length of the final DataFlash record. Download/file truncation or interrupted logging is possible; this does not prove onboard power loss.";
                    assessment.Confidence = "High";
                }
                else if (assessment.UnexpectedBytes > 0 || assessment.UnknownPacketTypes > 0)
                {
                    assessment.Status = "Uncertain";
                    assessment.Reason = "The binary scan encountered unexpected bytes or packet types whose length could not be verified. Corruption, custom records or parser/version differences remain possible.";
                    assessment.Confidence = "Medium";
                }
                else
                {
                    assessment.Status = "Normal";
                    assessment.Reason = "The physical binary tail ends on a complete declared DataFlash record.";
                    assessment.Confidence = "High";
                }

                if (parserRecordCount > 0 && assessment.CompleteRecords > 0 && Math.Abs(assessment.CompleteRecords - parserRecordCount) > Math.Max(5, parserRecordCount / 100))
                {
                    if (assessment.Status == "Normal") assessment.Status = "Uncertain";
                    assessment.Reason += " Physical record count and Mission Planner parser count differ materially, so parser recovery/custom data may be involved.";
                    assessment.Confidence = "Medium";
                }
            }
            return assessment;
        }

        public static void FinalizeWithState(LogAnalysisResult result)
        {
            if (result == null) return;
            if (result.Integrity == null) result.Integrity = new LogIntegrityAssessment();
            LogIntegrityAssessment assessment = result.Integrity;
            FlightStatePoint final = result.FlightStates == null || result.FlightStates.Count == 0 ? null : result.FlightStates[result.FlightStates.Count - 1];
            bool everAirborne = result.FlightStates != null && result.FlightStates.Any(s => s.Airborne);
            assessment.ArmedAtEnd = final != null && final.Armed;
            assessment.NoDisarmEvent = !result.Timeline.Any(t => t.Type == "Arming" && t.Text.IndexOf("disarmed", StringComparison.OrdinalIgnoreCase) >= 0);
            assessment.PowerRailEvidence = result.Findings.Any(f => (f.Subsystem ?? string.Empty).IndexOf("Power", StringComparison.OrdinalIgnoreCase) >= 0 || (f.Title ?? string.Empty).IndexOf("power", StringComparison.OrdinalIgnoreCase) >= 0 || (f.Evidence ?? string.Empty).IndexOf("Vcc", StringComparison.OrdinalIgnoreCase) >= 0);

            if (!string.IsNullOrEmpty(result.ParseError))
            {
                assessment.Status = "Incomplete";
                assessment.Reason = "The parser did not complete cleanly. " + result.ParseError;
                assessment.Confidence = "High";
                return;
            }
            if (assessment.Status == "Incomplete") return;

            if (everAirborne && assessment.ArmedAtEnd)
            {
                assessment.Status = "Abnormal";
                assessment.Reason = "The vehicle is still resolved as armed at the end of an airborne log. Logging may have stopped unexpectedly, the vehicle may not have disarmed, or the file may end before the normal shutdown sequence. Power loss is only one possibility.";
                assessment.Confidence = "High";
            }
            else if (everAirborne && assessment.NoDisarmEvent && final != null && !string.Equals(final.State, "Disarmed", StringComparison.OrdinalIgnoreCase))
            {
                if (assessment.Status == "Normal") assessment.Status = "Uncertain";
                assessment.Reason = "No explicit disarm record was found after airborne operation. A landed state may be present, but the final shutdown/disarm sequence is not fully proven.";
                assessment.Confidence = "Medium";
            }
            else if (assessment.Status == "Normal")
            {
                assessment.Reason = "Physical file structure is complete and the resolved ending does not show an armed airborne termination.";
            }
        }
    }

    internal sealed class NormalizedSignalSample
    {
        public double TimeMs;
        public string Signal;
        public double Value;
        public string Unit;
        public string Source;
        public string Phase;
    }

    internal static class NormalizedDataBuilder
    {
        public static Dictionary<string, List<NormalizedSignalSample>> Build(LogAnalysisResult result)
        {
            var output = new Dictionary<string, List<NormalizedSignalSample>>(StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, List<BatterySample>> pair in result.Batteries)
            {
                foreach (BatterySample s in pair.Value)
                {
                    Add(output, "battery.voltage", s.Voltage, "V", pair.Key, s.TimeMs, s.Phase);
                    Add(output, "battery.current", s.Current, "A", pair.Key, s.TimeMs, s.Phase);
                }
            }
            foreach (KeyValuePair<string, List<GpsSample>> pair in result.Gps)
            {
                foreach (GpsSample s in pair.Value)
                {
                    Add(output, "gps.speed", s.Speed, "m/s", pair.Key, s.TimeMs, s.Phase);
                    Add(output, "gps.altitude", s.Altitude, "m", pair.Key, s.TimeMs, s.Phase);
                    Add(output, "gps.hdop", s.Hdop, "", pair.Key, s.TimeMs, s.Phase);
                }
            }
            foreach (RcInputSample s in result.RcInputs)
            {
                Add(output, "pilot.roll", s.Roll, "%", "RCIN", s.TimeMs, s.Phase);
                Add(output, "pilot.pitch", s.Pitch, "%", "RCIN", s.TimeMs, s.Phase);
                Add(output, "pilot.throttle", s.Throttle, "%", "RCIN", s.TimeMs, s.Phase);
                Add(output, "pilot.yaw", s.Yaw, "%", "RCIN", s.TimeMs, s.Phase);
            }
            foreach (AttitudeSample s in result.Attitudes)
            {
                Add(output, "attitude.roll", s.Roll, "deg", "ATT", s.TimeMs, s.Phase);
                Add(output, "attitude.pitch", s.Pitch, "deg", "ATT", s.TimeMs, s.Phase);
                Add(output, "attitude.yaw", s.Yaw, "deg", "ATT", s.TimeMs, s.Phase);
                Add(output, "attitude.roll_error", s.RollError, "deg", "ATT", s.TimeMs, s.Phase);
                Add(output, "attitude.pitch_error", s.PitchError, "deg", "ATT", s.TimeMs, s.Phase);
                Add(output, "attitude.yaw_error", s.YawError, "deg", "ATT", s.TimeMs, s.Phase);
            }
            foreach (AltitudeSample s in result.Altitudes)
            {
                Add(output, "vertical.altitude", s.Altitude, "m", "CTUN", s.TimeMs, s.Phase);
                Add(output, "vertical.climb_rate", s.ClimbRate, "m/s", "CTUN", s.TimeMs, s.Phase);
            }
            foreach (RateSample s in result.Rates)
            {
                Add(output, "rate.roll_actual", s.RollActual, "deg/s", "RATE", s.TimeMs, s.Phase);
                Add(output, "rate.pitch_actual", s.PitchActual, "deg/s", "RATE", s.TimeMs, s.Phase);
                Add(output, "rate.yaw_actual", s.YawActual, "deg/s", "RATE", s.TimeMs, s.Phase);
            }
            return output;
        }

        private static void Add(Dictionary<string, List<NormalizedSignalSample>> output, string signal, double? value, string unit, string source, double timeMs, string phase)
        {
            if (!value.HasValue || double.IsNaN(value.Value) || double.IsInfinity(value.Value)) return;
            List<NormalizedSignalSample> list;
            if (!output.TryGetValue(signal, out list))
            {
                list = new List<NormalizedSignalSample>();
                output[signal] = list;
            }
            if (list.Count < 30000)
                list.Add(new NormalizedSignalSample { TimeMs = timeMs, Signal = signal, Value = value.Value, Unit = unit, Source = source, Phase = phase });
        }
    }

    internal sealed class PilotCommandPanel : Control
    {
        private LogAnalysisResult _result;
        private double _timeMs;
        private bool _hebrew;

        public PilotCommandPanel()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            BackColor = UiPalette.PlotSurface;
            ForeColor = UiPalette.Text;
            Font = new Font("Segoe UI", 8.8f);
        }

        public bool Hebrew
        {
            get { return _hebrew; }
            set { _hebrew = value; RightToLeft = RightToLeft.No; Invalidate(); }
        }

        public LogAnalysisResult Result
        {
            get { return _result; }
            set { _result = value; Invalidate(); }
        }

        public void SetTime(double timeMs)
        {
            _timeMs = timeMs;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(UiPalette.PlotSurface);

            using (var titleFont = new Font("Segoe UI Semibold", 11.5f, FontStyle.Bold))
            using (var titleBrush = new SolidBrush(UiPalette.TextStrong))
                g.DrawString(Localization.Ui("LIVE PILOT INPUT REPLAY", _hebrew), titleFont, titleBrush, 18, 13);

            if (_result == null)
            {
                DrawCentered(g, Localization.Ui("Open a log to inspect pilot commands.", _hebrew));
                return;
            }
            if (_result.RcInputs == null || _result.RcInputs.Count == 0)
            {
                DrawCentered(g, Localization.Ui("No RCIN records were found. Enable RC input logging to see stick movement.", _hebrew));
                return;
            }

            RcInputSample sample = FindNearest(_result.RcInputs, _timeMs <= 0 ? _result.FirstTimeMs : _timeMs);
            if (sample == null)
            {
                DrawCentered(g, Localization.Ui("No RC input sample is available at this time.", _hebrew));
                return;
            }

            int boxSize = Math.Max(120, Math.Min(190, Height - 105));
            int leftX = 35;
            int rightX = leftX + boxSize + 65;
            int top = 58;
            DrawStickBox(g, new Rectangle(leftX, top, boxSize, boxSize), Localization.Ui("LEFT STICK", _hebrew), Localization.Ui("Yaw", _hebrew), Localization.Ui("Throttle", _hebrew),
                sample.Yaw ?? 0, (sample.Throttle ?? 50) * 2 - 100, true);
            DrawStickBox(g, new Rectangle(rightX, top, boxSize, boxSize), Localization.Ui("RIGHT STICK", _hebrew), Localization.Ui("Roll", _hebrew), Localization.Ui("Pitch", _hebrew),
                sample.Roll ?? 0, sample.Pitch ?? 0, false);

            int infoX = rightX + boxSize + 40;
            int infoWidth = Math.Max(280, Width - infoX - 20);
            var infoRect = new Rectangle(infoX, top, infoWidth, boxSize);
            DrawInfo(g, infoRect, sample);
        }

        private void DrawStickBox(Graphics g, Rectangle rect, string title, string xLabel, string yLabel, double xValue, double yValue, bool throttle)
        {
            using (var fill = new SolidBrush(UiPalette.Surface)) g.FillRectangle(fill, rect);
            using (var border = new Pen(UiPalette.Border, 1.2f)) g.DrawRectangle(border, rect);
            using (var grid = new Pen(UiPalette.GridMajor, 1f))
            {
                g.DrawLine(grid, rect.Left + rect.Width / 2, rect.Top + 8, rect.Left + rect.Width / 2, rect.Bottom - 8);
                g.DrawLine(grid, rect.Left + 8, rect.Top + rect.Height / 2, rect.Right - 8, rect.Top + rect.Height / 2);
            }
            using (var font = new Font("Segoe UI Semibold", 9f, FontStyle.Bold))
            using (var brush = new SolidBrush(UiPalette.TextStrong))
                TextRenderer.DrawText(g, title, font, new Rectangle(rect.Left, rect.Top - 24, rect.Width, 22), UiPalette.TextStrong,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            double nx = Math.Max(-100, Math.Min(100, xValue));
            double ny = Math.Max(-100, Math.Min(100, yValue));
            float px = rect.Left + rect.Width / 2f + (float)(nx / 100.0 * (rect.Width / 2f - 14));
            float py = rect.Top + rect.Height / 2f - (float)(ny / 100.0 * (rect.Height / 2f - 14));
            using (var halo = new SolidBrush(Color.FromArgb(70, UiPalette.Accent))) g.FillEllipse(halo, px - 13, py - 13, 26, 26);
            using (var dot = new SolidBrush(UiPalette.Accent)) g.FillEllipse(dot, px - 7, py - 7, 14, 14);
            using (var outline = new Pen(UiPalette.TextStrong, 1f)) g.DrawEllipse(outline, px - 7, py - 7, 14, 14);

            using (var brush = new SolidBrush(UiPalette.Muted))
            {
                g.DrawString(xLabel + "  " + xValue.ToString("+0;-0;0", CultureInfo.InvariantCulture) + "%", Font, brush, rect.Left + 8, rect.Bottom - 22);
                string yText = throttle ? ((yValue + 100) / 2).ToString("0", CultureInfo.InvariantCulture) + "%" : yValue.ToString("+0;-0;0", CultureInfo.InvariantCulture) + "%";
                g.DrawString(yLabel + "  " + yText, Font, brush, rect.Left + 8, rect.Top + 7);
            }
        }

        private void DrawInfo(Graphics g, Rectangle rect, RcInputSample sample)
        {
            using (var fill = new SolidBrush(UiPalette.SurfaceAlt)) g.FillRectangle(fill, rect);
            using (var border = new Pen(UiPalette.Border)) g.DrawRectangle(border, rect);
            int y = rect.Top + 12;
            DrawInfoLine(g, rect.Left + 14, ref y, Localization.Ui("Time", _hebrew), FormatClock((_timeMs - _result.FirstTimeMs) / 1000.0), UiPalette.Advisory);
            DrawInfoLine(g, rect.Left + 14, ref y, Localization.Ui("Flight phase", _hebrew), _hebrew ? Localization.Phase(sample.Phase ?? "Unknown") : (sample.Phase ?? "Unknown"), UiPalette.Text);
            DrawInfoLine(g, rect.Left + 14, ref y, Localization.Ui("Roll", _hebrew), FormatInput(sample.Roll, sample.GetRaw(sample.RollChannel), sample.RollChannel), UiPalette.Text);
            DrawInfoLine(g, rect.Left + 14, ref y, Localization.Ui("Pitch", _hebrew), FormatInput(sample.Pitch, sample.GetRaw(sample.PitchChannel), sample.PitchChannel), UiPalette.Text);
            DrawInfoLine(g, rect.Left + 14, ref y, Localization.Ui("Throttle", _hebrew), FormatInput(sample.Throttle, sample.GetRaw(sample.ThrottleChannel), sample.ThrottleChannel), UiPalette.Text);
            DrawInfoLine(g, rect.Left + 14, ref y, Localization.Ui("Yaw", _hebrew), FormatInput(sample.Yaw, sample.GetRaw(sample.YawChannel), sample.YawChannel), UiPalette.Text);
            DrawInfoLine(g, rect.Left + 14, ref y, Localization.Ui("RC override mask", _hebrew), "0x" + sample.OverrideMask.ToString("X", CultureInfo.InvariantCulture),
                sample.PrimaryOverrideActive ? UiPalette.Warning : UiPalette.Success);
            if (sample.PrimaryOverrideActive)
            {
                using (var font = new Font("Segoe UI Semibold", 8.8f, FontStyle.Bold))
                using (var brush = new SolidBrush(UiPalette.Warning))
                    g.DrawString(_hebrew ? "MAVLink RC override פעיל בערוץ שליטה ראשי." : "MAVLink RC override is active on a primary control channel.", font, brush,
                        new RectangleF(rect.Left + 14, y + 4, rect.Width - 28, 38));
            }
            else
            {
                using (var brush = new SolidBrush(UiPalette.Muted))
                    g.DrawString(_hebrew ? "הערכים מנורמלים באמצעות RCMAP ופרמטרי כיול RCx כאשר הם זמינים." : "The values are normalized using RCMAP and RCx calibration parameters when available.", Font, brush,
                        new RectangleF(rect.Left + 14, y + 4, rect.Width - 28, 42));
            }
        }

        private void DrawInfoLine(Graphics g, int x, ref int y, string label, string value, Color valueColor)
        {
            using (var labelFont = new Font("Segoe UI Semibold", 8.7f, FontStyle.Bold))
            using (var labelBrush = new SolidBrush(UiPalette.Muted))
                g.DrawString(label + ":", labelFont, labelBrush, x, y);
            using (var valueBrush = new SolidBrush(valueColor))
                g.DrawString(value, Font, valueBrush, x + 112, y);
            y += 24;
        }

        private static string FormatInput(double? normalized, double? raw, int channel)
        {
            string value = normalized.HasValue ? normalized.Value.ToString("+0.0;-0.0;0.0", CultureInfo.InvariantCulture) + "%" : "n/a";
            string rawValue = raw.HasValue ? raw.Value.ToString("0", CultureInfo.InvariantCulture) + " µs" : "n/a";
            return value + "  (RC" + channel.ToString(CultureInfo.InvariantCulture) + " " + rawValue + ")";
        }

        private static RcInputSample FindNearest(List<RcInputSample> samples, double timeMs)
        {
            if (samples == null || samples.Count == 0) return null;
            int low = 0, high = samples.Count - 1;
            while (low <= high)
            {
                int mid = low + (high - low) / 2;
                if (samples[mid].TimeMs < timeMs) low = mid + 1;
                else if (samples[mid].TimeMs > timeMs) high = mid - 1;
                else return samples[mid];
            }
            int a = Math.Max(0, Math.Min(samples.Count - 1, low));
            int b = Math.Max(0, Math.Min(samples.Count - 1, low - 1));
            return Math.Abs(samples[a].TimeMs - timeMs) < Math.Abs(samples[b].TimeMs - timeMs) ? samples[a] : samples[b];
        }

        private static string FormatClock(double seconds)
        {
            if (seconds < 0) seconds = 0;
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            return string.Format(CultureInfo.InvariantCulture, "{0:00}:{1:00}:{2:00}.{3:00}",
                (int)time.TotalHours, time.Minutes, time.Seconds, time.Milliseconds / 10);
        }

        private void DrawCentered(Graphics g, string text)
        {
            SizeF size = g.MeasureString(text, Font);
            using (var brush = new SolidBrush(UiPalette.Muted))
                g.DrawString(text, Font, brush, Math.Max(12, (Width - size.Width) / 2f), Math.Max(50, (Height - size.Height) / 2f));
        }
    }

    internal sealed class InvestigatorGraphControl : Control
    {
        private LogAnalysisResult _result;
        private string _graphName = "Battery voltage/current";
        private double _highlightTimeMs;
        private List<GraphSeries> _series = new List<GraphSeries>();
        private readonly HashSet<string> _hiddenSeries = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly List<GraphLegendHit> _legendHits = new List<GraphLegendHit>();
        private Rectangle _plot;
        private double _viewMinX;
        private double _viewMaxX = 1;
        private bool _viewInitialized;
        private bool _panning;
        private bool _dragMoved;
        private Point _mouseDown;
        private double _panStartMinX;
        private double _panStartMaxX;
        private bool _hasHover;
        private double _hoverX;
        private Point _hoverPoint;

        public event EventHandler<MapTimeChangedEventArgs> TimeSelected;

        public InvestigatorGraphControl()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            BackColor = UiPalette.AppBackground;
            ForeColor = UiPalette.Text;
            Font = new Font("Segoe UI", 8.6f);
            TabStop = true;
            SetStyle(ControlStyles.Selectable, true);
        }

        public LogAnalysisResult Result
        {
            get { return _result; }
            set
            {
                if (ReferenceEquals(_result, value))
                    return;
                _result = value;
                RebuildSeries(true);
            }
        }

        public string GraphName
        {
            get { return _graphName; }
            set
            {
                string next = value ?? string.Empty;
                if (string.Equals(_graphName, next, StringComparison.Ordinal))
                    return;
                _graphName = next;
                RebuildSeries(true);
            }
        }

        public double HighlightTimeMs
        {
            get { return _highlightTimeMs; }
            set
            {
                _highlightTimeMs = value;
                Invalidate();
            }
        }

        public void ResetView()
        {
            if (_result == null)
            {
                _viewMinX = 0;
                _viewMaxX = 1;
            }
            else
            {
                _viewMinX = 0;
                _viewMaxX = Math.Max(1, _result.DurationSeconds);
            }
            _viewInitialized = true;
            Invalidate();
        }

        public void ZoomBy(double factor)
        {
            if (_result == null || factor <= 0)
                return;
            EnsureView();
            double center = (_viewMinX + _viewMaxX) / 2.0;
            ZoomAround(center, factor);
        }

        public void ShowAllSeries()
        {
            _hiddenSeries.Clear();
            Invalidate();
        }

        private void RebuildSeries(bool resetView)
        {
            _series = _result == null ? new List<GraphSeries>() : BuildSeries(_result, _graphName);
            _hiddenSeries.Clear();
            _hasHover = false;
            if (resetView)
                _viewInitialized = false;
            Invalidate();
        }

        private void EnsureView()
        {
            if (_viewInitialized)
                return;
            ResetView();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(UiPalette.AppBackground);
            _plot = new Rectangle(76, 76, Math.Max(80, ClientSize.Width - 116), Math.Max(80, ClientSize.Height - 150));
            DrawFrame(g, _plot);

            using (var titleFont = new Font("Segoe UI Semibold", 13.0f, FontStyle.Bold))
            using (var titleBrush = new SolidBrush(UiPalette.Text))
                g.DrawString(_graphName, titleFont, titleBrush, _plot.Left, 14);

            if (_result == null)
            {
                DrawCentered(g, "No log selected", _plot);
                return;
            }

            EnsureView();
            List<GraphSeries> visible = _series.Where(s => !_hiddenSeries.Contains(s.Name)).ToList();
            DrawLegend(g, _plot, _series);

            if (_series.Count == 0 || _series.All(s => s.Points.Count == 0))
            {
                DrawCentered(g, "No suitable data available for " + _graphName, _plot);
                return;
            }
            if (visible.Count == 0)
            {
                DrawCentered(g, "All series are hidden. Click a legend item or Show all.", _plot);
                return;
            }

            double minX = _viewMinX;
            double maxX = _viewMaxX;
            List<GraphPoint> inView = visible.SelectMany(s => s.Points.Where(p => p.X >= minX && p.X <= maxX)).ToList();
            if (inView.Count == 0)
                inView = visible.SelectMany(s => s.Points).ToList();
            double minY = inView.Min(p => p.Y);
            double maxY = inView.Max(p => p.Y);
            if (Math.Abs(maxY - minY) < 1e-9) { minY -= 1; maxY += 1; }
            double padding = (maxY - minY) * 0.08;
            minY -= padding;
            maxY += padding;

            DrawAxes(g, _plot, minX, maxX, minY, maxY);
            DrawSeries(g, _plot, visible, minX, maxX, minY, maxY);
            DrawEventMarkers(g, _plot, minX, maxX);
            DrawHover(g, _plot, visible, minX, maxX, minY, maxY);

            using (var textBrush = new SolidBrush(UiPalette.Text))
                g.DrawString("Time (seconds)", Font, textBrush, _plot.Left + _plot.Width / 2 - 42, _plot.Bottom + 35);
            using (var mutedBrush = new SolidBrush(UiPalette.Muted))
            {
                g.DrawString("Wheel = zoom  |  Drag = pan  |  Click = seek map/replay  |  Double-click = reset  |  Click legend = hide/show",
                    Font, mutedBrush, _plot.Left, _plot.Bottom + 55);
                g.DrawString("Created by Nadav Golan-Yanay  •  © 2026 All rights reserved.",
                    Font, mutedBrush, Math.Max(_plot.Left, _plot.Right - 420), _plot.Bottom + 75);
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (_result == null || !_plot.Contains(e.Location))
                return;
            Focus();
            EnsureView();
            double anchor = PixelToX(e.X);
            ZoomAround(anchor, e.Delta > 0 ? 0.72 : 1.38);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
            if (e.Button != MouseButtons.Left)
                return;

            GraphLegendHit hit = _legendHits.FirstOrDefault(h => h.Bounds.Contains(e.Location));
            if (hit != null)
            {
                if (_hiddenSeries.Contains(hit.Name)) _hiddenSeries.Remove(hit.Name);
                else _hiddenSeries.Add(hit.Name);
                Invalidate();
                return;
            }

            if (!_plot.Contains(e.Location) || _result == null)
                return;
            _panning = true;
            _dragMoved = false;
            _mouseDown = e.Location;
            _panStartMinX = _viewMinX;
            _panStartMaxX = _viewMaxX;
            Cursor = Cursors.SizeWE;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_panning && e.Button == MouseButtons.Left)
            {
                int deltaPixels = e.X - _mouseDown.X;
                if (Math.Abs(deltaPixels) > 3) _dragMoved = true;
                double span = _panStartMaxX - _panStartMinX;
                double shift = -deltaPixels * span / Math.Max(1, _plot.Width);
                SetView(_panStartMinX + shift, _panStartMaxX + shift);
                return;
            }

            if (_plot.Contains(e.Location) && _result != null)
            {
                _hasHover = true;
                _hoverX = PixelToX(e.X);
                _hoverPoint = e.Location;
                Cursor = Cursors.Cross;
                Invalidate();
            }
            else
            {
                _hasHover = false;
                Cursor = Cursors.Default;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (!_panning || e.Button != MouseButtons.Left)
                return;
            _panning = false;
            Cursor = Cursors.Cross;
            if (!_dragMoved && _plot.Contains(e.Location) && _result != null)
            {
                double seconds = PixelToX(e.X);
                double timeMs = _result.FirstTimeMs + seconds * 1000.0;
                HighlightTimeMs = timeMs;
                if (TimeSelected != null)
                    TimeSelected(this, new MapTimeChangedEventArgs(timeMs));
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (_plot.Contains(e.Location))
                ResetView();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (!_panning)
            {
                _hasHover = false;
                Cursor = Cursors.Default;
                Invalidate();
            }
        }

        private void ZoomAround(double anchor, double factor)
        {
            EnsureView();
            double duration = Math.Max(1, _result.DurationSeconds);
            double oldSpan = _viewMaxX - _viewMinX;
            double newSpan = Math.Max(0.25, Math.Min(duration, oldSpan * factor));
            double ratio = oldSpan <= 0 ? 0.5 : (anchor - _viewMinX) / oldSpan;
            double min = anchor - newSpan * ratio;
            SetView(min, min + newSpan);
        }

        private void SetView(double min, double max)
        {
            double duration = _result == null ? 1 : Math.Max(1, _result.DurationSeconds);
            double span = Math.Max(0.25, Math.Min(duration, max - min));
            if (min < 0) min = 0;
            if (min + span > duration) min = duration - span;
            _viewMinX = Math.Max(0, min);
            _viewMaxX = _viewMinX + span;
            _viewInitialized = true;
            Invalidate();
        }

        private double PixelToX(int pixelX)
        {
            double fraction = (double)(pixelX - _plot.Left) / Math.Max(1, _plot.Width);
            fraction = Math.Max(0, Math.Min(1, fraction));
            return _viewMinX + fraction * (_viewMaxX - _viewMinX);
        }

        private int XToPixel(double x)
        {
            return _plot.Left + (int)((x - _viewMinX) / Math.Max(1e-9, _viewMaxX - _viewMinX) * _plot.Width);
        }

        private void DrawFrame(Graphics g, Rectangle plot)
        {
            using (var background = new SolidBrush(UiPalette.PlotSurface))
                g.FillRectangle(background, plot);
            using (var grid = new Pen(UiPalette.Grid))
            {
                for (int i = 0; i <= 10; i++)
                {
                    int x = plot.Left + plot.Width * i / 10;
                    int y = plot.Top + plot.Height * i / 10;
                    g.DrawLine(grid, x, plot.Top, x, plot.Bottom);
                    g.DrawLine(grid, plot.Left, y, plot.Right, y);
                }
            }
            using (var border = new Pen(UiPalette.Border, 1.2f))
                g.DrawRectangle(border, plot);
        }

        private void DrawAxes(Graphics g, Rectangle plot, double minX, double maxX, double minY, double maxY)
        {
            for (int i = 0; i <= 5; i++)
            {
                double xVal = minX + (maxX - minX) * i / 5.0;
                double yVal = maxY - (maxY - minY) * i / 5.0;
                int x = plot.Left + plot.Width * i / 5;
                int y = plot.Top + plot.Height * i / 5;
                using (var axisBrush = new SolidBrush(UiPalette.Muted))
                {
                    g.DrawString(xVal.ToString("0.0", CultureInfo.InvariantCulture), Font, axisBrush, x - 14, plot.Bottom + 5);
                    g.DrawString(yVal.ToString("0.##", CultureInfo.InvariantCulture), Font, axisBrush, 5, y - 7);
                }
            }
        }

        private void DrawSeries(Graphics g, Rectangle plot, List<GraphSeries> series, double minX, double maxX, double minY, double maxY)
        {
            foreach (var item in series)
            {
                if (item.Points.Count < 2) continue;
                using (var pen = new Pen(item.Color, 2f))
                {
                    PointF? previous = null;
                    foreach (var point in item.Points)
                    {
                        if (point.X < minX || point.X > maxX) { previous = null; continue; }
                        float x = plot.Left + (float)((point.X - minX) / (maxX - minX) * plot.Width);
                        float y = plot.Bottom - (float)((point.Y - minY) / (maxY - minY) * plot.Height);
                        var current = new PointF(x, y);
                        if (previous.HasValue) g.DrawLine(pen, previous.Value, current);
                        previous = current;
                    }
                }
            }
        }

        private void DrawLegend(Graphics g, Rectangle plot, List<GraphSeries> series)
        {
            _legendHits.Clear();
            int x = plot.Left;
            int y = 34;
            foreach (var item in series)
            {
                bool hidden = _hiddenSeries.Contains(item.Name);
                int textWidth = (int)Math.Ceiling(g.MeasureString(item.Name, Font).Width);
                var bounds = new Rectangle(x - 3, y - 2, textWidth + 52, 20);
                _legendHits.Add(new GraphLegendHit { Name = item.Name, Bounds = bounds });
                using (var pen = new Pen(hidden ? UiPalette.Disabled : item.Color, 3f))
                    g.DrawLine(pen, x, y + 7, x + 20, y + 7);
                using (var legendBrush = new SolidBrush(hidden ? UiPalette.Disabled : UiPalette.Text))
                    g.DrawString(item.Name, Font, legendBrush, x + 24, y);
                if (hidden)
                    using (var disabledPen = new Pen(UiPalette.Disabled))
                        g.DrawLine(disabledPen, x + 24, y + 8, x + 24 + textWidth, y + 8);
                x += textWidth + 54;
                if (x > plot.Right - 150) { x = plot.Left; y += 20; }
            }
        }

        private void DrawEventMarkers(Graphics g, Rectangle plot, double minX, double maxX)
        {
            if (_result == null || maxX <= minX) return;
            foreach (var finding in _result.Findings.Where(f => f.Severity == "Critical" || f.Severity == "Warning").Take(30))
            {
                double seconds = (finding.TimeMs - _result.FirstTimeMs) / 1000.0;
                if (seconds < minX || seconds > maxX) continue;
                int x = plot.Left + (int)((seconds - minX) / (maxX - minX) * plot.Width);
                using (var pen = new Pen(finding.Severity == "Critical" ? UiPalette.Critical : UiPalette.Warning, 1f))
                    g.DrawLine(pen, x, plot.Top, x, plot.Bottom);
            }

            foreach (var command in _result.MavCommands.Take(50))
            {
                double seconds = (command.TimeMs - _result.FirstTimeMs) / 1000.0;
                if (seconds < minX || seconds > maxX) continue;
                int x = plot.Left + (int)((seconds - minX) / (maxX - minX) * plot.Width);
                using (var pen = new Pen(Color.FromArgb(150, UiPalette.Advisory), 1f))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    g.DrawLine(pen, x, plot.Top, x, plot.Bottom);
                }
            }

            if (_highlightTimeMs > 0)
            {
                double seconds = (_highlightTimeMs - _result.FirstTimeMs) / 1000.0;
                if (seconds >= minX && seconds <= maxX)
                {
                    int x = plot.Left + (int)((seconds - minX) / (maxX - minX) * plot.Width);
                    using (var pen = new Pen(UiPalette.Replay, 3f)) g.DrawLine(pen, x, plot.Top, x, plot.Bottom);
                    using (var replayBrush = new SolidBrush(UiPalette.Replay))
                        g.DrawString("Replay", Font, replayBrush, Math.Min(x + 4, plot.Right - 48), plot.Top + 4);
                }
            }
        }

        private void DrawHover(Graphics g, Rectangle plot, List<GraphSeries> visible, double minX, double maxX, double minY, double maxY)
        {
            if (!_hasHover || !_plot.Contains(_hoverPoint))
                return;
            int x = XToPixel(_hoverX);
            using (var pen = new Pen(Color.FromArgb(170, UiPalette.Muted), 1f))
                g.DrawLine(pen, x, plot.Top, x, plot.Bottom);

            var values = new List<string>();
            values.Add("Time: " + FormatClock(_hoverX));
            foreach (var series in visible)
            {
                GraphPoint nearest = FindNearest(series.Points, _hoverX);
                if (nearest == null) continue;
                int y = plot.Bottom - (int)((nearest.Y - minY) / Math.Max(1e-9, maxY - minY) * plot.Height);
                using (var brush = new SolidBrush(series.Color)) g.FillEllipse(brush, x - 4, y - 4, 8, 8);
                values.Add(series.Name + ": " + nearest.Y.ToString("0.###", CultureInfo.InvariantCulture));
            }

            string text = string.Join("\n", values.ToArray());
            SizeF size = g.MeasureString(text, Font);
            int boxX = Math.Min(plot.Right - (int)size.Width - 16, x + 12);
            if (boxX < plot.Left) boxX = plot.Left;
            int boxY = Math.Max(plot.Top, Math.Min(plot.Bottom - (int)size.Height - 12, _hoverPoint.Y + 12));
            var box = new Rectangle(boxX, boxY, (int)size.Width + 12, (int)size.Height + 8);
            using (var brush = new SolidBrush(UiPalette.TooltipBackground)) g.FillRectangle(brush, box);
            using (var borderPen = new Pen(UiPalette.TooltipBorder)) g.DrawRectangle(borderPen, box);
            using (var textBrush = new SolidBrush(UiPalette.TextStrong)) g.DrawString(text, Font, textBrush, box.Left + 6, box.Top + 4);
        }

        private static GraphPoint FindNearest(List<GraphPoint> points, double x)
        {
            if (points == null || points.Count == 0) return null;
            int low = 0, high = points.Count - 1;
            while (low <= high)
            {
                int mid = low + (high - low) / 2;
                if (points[mid].X < x) low = mid + 1;
                else if (points[mid].X > x) high = mid - 1;
                else return points[mid];
            }
            int a = Math.Max(0, Math.Min(points.Count - 1, low));
            int b = Math.Max(0, Math.Min(points.Count - 1, low - 1));
            return Math.Abs(points[a].X - x) < Math.Abs(points[b].X - x) ? points[a] : points[b];
        }

        private static string FormatClock(double seconds)
        {
            if (seconds < 0) seconds = 0;
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            return string.Format(CultureInfo.InvariantCulture, "{0:00}:{1:00}:{2:00}.{3:00}",
                (int)time.TotalHours, time.Minutes, time.Seconds, time.Milliseconds / 10);
        }

        private static List<GraphSeries> BuildSeries(LogAnalysisResult result, string graphName)
        {
            var colors = new[] { Color.FromArgb(91, 174, 255), Color.FromArgb(255, 184, 92), Color.FromArgb(92, 222, 153), Color.FromArgb(195, 137, 255), Color.FromArgb(255, 112, 126), Color.FromArgb(91, 220, 235), Color.FromArgb(120, 145, 255), Color.FromArgb(239, 141, 219) };
            var output = new List<GraphSeries>();
            if (graphName == "Battery voltage/current")
            {
                var data = result.Batteries.OrderByDescending(p => p.Value.Count).Select(p => p.Value).FirstOrDefault();
                if (data != null)
                {
                    output.Add(MakeSeries("Voltage", colors[0], data.Where(s => s.Voltage.HasValue).Select(s => Pair(result, s.TimeMs, s.Voltage.Value))));
                    output.Add(MakeSeries("Current", colors[1], data.Where(s => s.Current.HasValue).Select(s => Pair(result, s.TimeMs, s.Current.Value))));
                }
            }
            else if (graphName == "Vibration and clipping")
            {
                output.Add(MakeSeries("Vibe X", colors[0], result.Vibes.Select(s => Pair(result, s.TimeMs, s.X))));
                output.Add(MakeSeries("Vibe Y", colors[1], result.Vibes.Select(s => Pair(result, s.TimeMs, s.Y))));
                output.Add(MakeSeries("Vibe Z", colors[2], result.Vibes.Select(s => Pair(result, s.TimeMs, s.Z))));
                output.Add(MakeSeries("Total clips", colors[3], result.Vibes.Select(s => Pair(result, s.TimeMs, s.Clip0 + s.Clip1 + s.Clip2))));
            }
            else if (graphName == "Attitude error")
            {
                output.Add(MakeSeries("Roll error", colors[0], result.Attitudes.Where(s => s.RollError.HasValue).Select(s => Pair(result, s.TimeMs, s.RollError.Value))));
                output.Add(MakeSeries("Pitch error", colors[1], result.Attitudes.Where(s => s.PitchError.HasValue).Select(s => Pair(result, s.TimeMs, s.PitchError.Value))));
                output.Add(MakeSeries("Yaw error", colors[2], result.Attitudes.Where(s => s.YawError.HasValue).Select(s => Pair(result, s.TimeMs, s.YawError.Value))));
            }
            else if (graphName == "Altitude and climb")
            {
                output.Add(MakeSeries("Altitude", colors[0], result.Altitudes.Where(s => s.Altitude.HasValue).Select(s => Pair(result, s.TimeMs, s.Altitude.Value))));
                output.Add(MakeSeries("Desired alt", colors[1], result.Altitudes.Where(s => s.DesiredAltitude.HasValue).Select(s => Pair(result, s.TimeMs, s.DesiredAltitude.Value))));
                output.Add(MakeSeries("Climb rate", colors[2], result.Altitudes.Where(s => s.ClimbRate.HasValue).Select(s => Pair(result, s.TimeMs, s.ClimbRate.Value))));
            }
            else if (graphName == "Motor outputs")
            {
                var channels = result.Motors.Where(m => m.Values != null).SelectMany(m => m.Values.Keys).Distinct().OrderBy(x => x).Take(8).ToList();
                for (int i = 0; i < channels.Count; i++)
                {
                    int ch = channels[i];
                    output.Add(MakeSeries("Ch" + ch.ToString(CultureInfo.InvariantCulture), colors[i % colors.Length],
                        result.Motors.Where(m => m.Values != null && m.Values.ContainsKey(ch)).Select(m => Pair(result, m.TimeMs, m.Values[ch]))));
                }
            }
            else if (graphName == "Pilot stick commands")
            {
                output.Add(MakeSeries("Roll %", colors[0], result.RcInputs.Where(s => s.Roll.HasValue).Select(s => Pair(result, s.TimeMs, s.Roll.Value))));
                output.Add(MakeSeries("Pitch %", colors[1], result.RcInputs.Where(s => s.Pitch.HasValue).Select(s => Pair(result, s.TimeMs, s.Pitch.Value))));
                output.Add(MakeSeries("Throttle %", colors[2], result.RcInputs.Where(s => s.Throttle.HasValue).Select(s => Pair(result, s.TimeMs, s.Throttle.Value))));
                output.Add(MakeSeries("Yaw %", colors[3], result.RcInputs.Where(s => s.Yaw.HasValue).Select(s => Pair(result, s.TimeMs, s.Yaw.Value))));
            }
            else if (graphName == "EKF health")
            {
                output.Add(MakeSeries("Vel var", colors[0], result.Ekf.Where(s => s.VelocityVariance.HasValue).Select(s => Pair(result, s.TimeMs, s.VelocityVariance.Value))));
                output.Add(MakeSeries("Pos var", colors[1], result.Ekf.Where(s => s.PositionVariance.HasValue).Select(s => Pair(result, s.TimeMs, s.PositionVariance.Value))));
                output.Add(MakeSeries("Hgt var", colors[2], result.Ekf.Where(s => s.HeightVariance.HasValue).Select(s => Pair(result, s.TimeMs, s.HeightVariance.Value))));
                output.Add(MakeSeries("Mag var", colors[3], result.Ekf.Where(s => s.MagneticVariance.HasValue).Select(s => Pair(result, s.TimeMs, s.MagneticVariance.Value))));
            }
            else if (graphName == "GPS quality")
            {
                var data = result.Gps.OrderByDescending(p => p.Value.Count).Select(p => p.Value).FirstOrDefault();
                if (data != null)
                {
                    output.Add(MakeSeries("Satellites", colors[0], data.Where(s => s.Sats.HasValue).Select(s => Pair(result, s.TimeMs, s.Sats.Value))));
                    output.Add(MakeSeries("Fix type", colors[1], data.Where(s => s.Status.HasValue).Select(s => Pair(result, s.TimeMs, s.Status.Value))));
                    output.Add(MakeSeries("HDOP", colors[2], data.Where(s => s.Hdop.HasValue).Select(s => Pair(result, s.TimeMs, s.Hdop.Value))));
                }
            }
            return output.Where(s => s.Points.Count > 0).ToList();
        }

        private static GraphPoint Pair(LogAnalysisResult result, double timeMs, double value)
        {
            return new GraphPoint { X = (timeMs - result.FirstTimeMs) / 1000.0, Y = value };
        }

        private static GraphSeries MakeSeries(string name, Color color, IEnumerable<GraphPoint> points)
        {
            var list = points.OrderBy(p => p.X).ToList();
            if (list.Count > 2500)
            {
                var reduced = new List<GraphPoint>();
                double step = (double)list.Count / 2500.0;
                for (int i = 0; i < 2500; i++) reduced.Add(list[Math.Min(list.Count - 1, (int)(i * step))]);
                list = reduced;
            }
            return new GraphSeries { Name = name, Color = color, Points = list };
        }

        private static void DrawCentered(Graphics g, string text, Rectangle rect)
        {
            var size = g.MeasureString(text, SystemFonts.DefaultFont);
            using (var brush = new SolidBrush(UiPalette.Muted))
                g.DrawString(text, SystemFonts.DefaultFont, brush,
                    rect.Left + (rect.Width - size.Width) / 2f, rect.Top + (rect.Height - size.Height) / 2f);
        }
    }

    internal sealed class GraphLegendHit
    {
        public string Name;
        public Rectangle Bounds;
    }

    internal sealed class GraphSeries
    {
        public string Name;
        public Color Color;
        public List<GraphPoint> Points = new List<GraphPoint>();
    }

    internal sealed class GraphPoint
    {
        public double X;
        public double Y;
    }

    internal sealed class MapTimeChangedEventArgs : EventArgs
    {
        public double TimeMs { get; private set; }

        public MapTimeChangedEventArgs(double timeMs)
        {
            TimeMs = timeMs;
        }
    }

    internal sealed class InteractiveMapPanel : UserControl
    {
        private sealed class GpsTrackOption
        {
            public string Key;
            public string Label;
            public bool ShowAll;
            public override string ToString() { return Label ?? string.Empty; }
        }

        private readonly InteractiveFlightMapControl _map;
        private readonly TrackBar _timeline;
        private readonly Button _playButton;
        private readonly Button _reverseButton;
        private readonly Button _stepBackButton;
        private readonly Button _stepForwardButton;
        private readonly Button _fitButton;
        private readonly Button _rangeStartButton;
        private readonly Button _rangeEndButton;
        private readonly Button _clearRangeButton;
        private readonly ComboBox _speedSelector;
        private readonly ComboBox _gpsSelector;
        private readonly CheckBox _followCheck;
        private readonly CheckBox _eventsCheck;
        private readonly CheckBox _futureCheck;
        private readonly CheckBox _waypointsCheck;
        private readonly CheckBox _modesCheck;
        private readonly CheckBox _referencesCheck;
        private readonly CheckBox _rangeOnlyCheck;
        private readonly Label _rangeLabel;
        private readonly Label _timeLabel;
        private readonly Label _dataLabel;
        private readonly Timer _playTimer;
        private readonly ToolTip _toolTip;
        private LogAnalysisResult _result;
        private double _currentTimeMs;
        private double? _rangeStartMs;
        private double? _rangeEndMs;
        private bool _playing;
        private int _direction = 1;
        private bool _updatingTimeline;
        private bool _updatingGpsSelector;
        private DateTime _lastTickUtc;

        public event EventHandler<MapTimeChangedEventArgs> TimeChanged;
        public double CurrentTimeMs { get { return _currentTimeMs; } }

        public InteractiveMapPanel()
        {
            _map = new InteractiveFlightMapControl();
            _map.Dock = DockStyle.Fill;
            _map.TimeSelected += Map_TimeSelected;

            var toolbarHost = new Panel();
            toolbarHost.Dock = DockStyle.Top;
            toolbarHost.Height = 88;
            toolbarHost.BackColor = UiPalette.Surface;

            var toolbar = new FlowLayoutPanel();
            toolbar.Location = new Point(0, 0);
            toolbar.Size = new Size(3000, 44);
            toolbar.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            toolbar.Padding = new Padding(12, 6, 12, 5);
            toolbar.WrapContents = false;
            toolbar.BackColor = UiPalette.Surface;
            toolbar.BorderStyle = BorderStyle.None;

            var layerToolbar = new FlowLayoutPanel();
            layerToolbar.Location = new Point(0, 44);
            layerToolbar.Size = new Size(3000, 44);
            layerToolbar.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            layerToolbar.Padding = new Padding(12, 5, 12, 5);
            layerToolbar.WrapContents = false;
            layerToolbar.AutoScroll = true;
            layerToolbar.BackColor = UiPalette.SurfaceAlt;
            layerToolbar.BorderStyle = BorderStyle.None;

            _playButton = MakeButton("Play", PlayButton_Click, 68);
            _reverseButton = MakeButton("Reverse", ReverseButton_Click, 82);
            _stepBackButton = MakeButton("-1 s", StepBackButton_Click, 68);
            _stepForwardButton = MakeButton("+1 s", StepForwardButton_Click, 68);
            _fitButton = MakeButton("Fit track", FitButton_Click, 100);

            _speedSelector = new ComboBox();
            _speedSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            _speedSelector.Width = 72;
            UiPalette.StyleCombo(_speedSelector);
            _speedSelector.Items.AddRange(new object[] { "0.25x", "0.5x", "1x", "2x", "5x", "10x" });
            _speedSelector.SelectedIndex = 2;

            _followCheck = MakeCheckBox("Follow vehicle", false);
            _eventsCheck = MakeCheckBox("Events", true);
            _futureCheck = MakeCheckBox("Future path", true);
            _eventsCheck.CheckedChanged += delegate { _map.ShowEvents = _eventsCheck.Checked; _map.Invalidate(); };
            _futureCheck.CheckedChanged += delegate { _map.ShowFuturePath = _futureCheck.Checked; _map.Invalidate(); };

            toolbar.Controls.Add(_playButton);
            toolbar.Controls.Add(_reverseButton);
            toolbar.Controls.Add(_stepBackButton);
            toolbar.Controls.Add(_stepForwardButton);
            toolbar.Controls.Add(_fitButton);
            toolbar.Controls.Add(MakeLabel("Speed:"));
            toolbar.Controls.Add(_speedSelector);
            toolbar.Controls.Add(_followCheck);
            toolbar.Controls.Add(_eventsCheck);
            toolbar.Controls.Add(_futureCheck);

            _gpsSelector = new ComboBox();
            _gpsSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            _gpsSelector.Width = 275;
            UiPalette.StyleCombo(_gpsSelector);
            _gpsSelector.SelectedIndexChanged += GpsSelector_SelectedIndexChanged;

            _waypointsCheck = MakeCheckBox("Waypoints", true);
            _modesCheck = MakeCheckBox("Mode changes", true);
            _referencesCheck = MakeCheckBox("References", true);
            _waypointsCheck.CheckedChanged += delegate { _map.ShowWaypoints = _waypointsCheck.Checked; _map.Invalidate(); };
            _modesCheck.CheckedChanged += delegate { _map.ShowModes = _modesCheck.Checked; _map.Invalidate(); };
            _referencesCheck.CheckedChanged += delegate { _map.ShowReferences = _referencesCheck.Checked; _map.Invalidate(); };

            _rangeStartButton = MakeButton("Set start", RangeStartButton_Click, 82);
            _rangeEndButton = MakeButton("Set end", RangeEndButton_Click, 78);
            _clearRangeButton = MakeButton("Clear range", ClearRangeButton_Click, 92);
            _rangeOnlyCheck = MakeCheckBox("Range only", false);
            _rangeOnlyCheck.Enabled = false;
            _rangeOnlyCheck.CheckedChanged += RangeOnlyCheck_CheckedChanged;

            _rangeLabel = new Label();
            _rangeLabel.AutoSize = false;
            _rangeLabel.Size = new Size(315, 30);
            _rangeLabel.TextAlign = ContentAlignment.MiddleLeft;
            _rangeLabel.ForeColor = UiPalette.Muted;
            _rangeLabel.Font = new Font("Segoe UI", 8.5f);
            _rangeLabel.Text = "Selected range: full log";

            layerToolbar.Controls.Add(MakeLabel("GPS:"));
            layerToolbar.Controls.Add(_gpsSelector);
            layerToolbar.Controls.Add(_waypointsCheck);
            layerToolbar.Controls.Add(_modesCheck);
            layerToolbar.Controls.Add(_referencesCheck);
            layerToolbar.Controls.Add(_rangeStartButton);
            layerToolbar.Controls.Add(_rangeEndButton);
            layerToolbar.Controls.Add(_clearRangeButton);
            layerToolbar.Controls.Add(_rangeOnlyCheck);
            layerToolbar.Controls.Add(_rangeLabel);

            toolbarHost.Controls.Add(layerToolbar);
            toolbarHost.Controls.Add(toolbar);

            _toolTip = new ToolTip();
            _toolTip.AutoPopDelay = 8000;
            _toolTip.InitialDelay = 400;
            _toolTip.ReshowDelay = 100;
            _toolTip.SetToolTip(_playButton, "Play the flight forward from the current time.");
            _toolTip.SetToolTip(_reverseButton, "Play the flight backward from the current time.");
            _toolTip.SetToolTip(_stepBackButton, "Move the replay one second backward.");
            _toolTip.SetToolTip(_stepForwardButton, "Move the replay one second forward.");
            _toolTip.SetToolTip(_fitButton, "Reset pan and zoom to the selected GPS view and selected time range.");
            _toolTip.SetToolTip(_gpsSelector, "Choose the GPS receiver used for the vehicle cursor, or overlay all logged receivers. Receiver type comes from GPS_TYPE/GPSx_TYPE when available.");
            _toolTip.SetToolTip(_rangeStartButton, "Use the current replay cursor as the beginning of a time segment.");
            _toolTip.SetToolTip(_rangeEndButton, "Use the current replay cursor as the end of a time segment.");
            _toolTip.SetToolTip(_rangeOnlyCheck, "Restrict replay, timeline and visible GPS track to the selected time segment without modifying the original log.");

            var bottom = new Panel();
            bottom.Dock = DockStyle.Bottom;
            bottom.Height = 78;
            bottom.Padding = new Padding(12, 3, 12, 6);
            bottom.BackColor = UiPalette.Surface;
            bottom.BorderStyle = BorderStyle.None;

            _timeline = new TrackBar();
            _timeline.Dock = DockStyle.Top;
            _timeline.Height = 35;
            _timeline.Minimum = 0;
            _timeline.Maximum = 10000;
            _timeline.TickStyle = TickStyle.None;
            _timeline.BackColor = UiPalette.Surface;
            _timeline.Enabled = false;
            _timeline.Scroll += Timeline_Scroll;

            _timeLabel = new Label();
            _timeLabel.Dock = DockStyle.Left;
            _timeLabel.Width = 250;
            _timeLabel.TextAlign = ContentAlignment.MiddleLeft;
            _timeLabel.ForeColor = UiPalette.Advisory;
            _timeLabel.Font = new Font("Segoe UI Semibold", 9.0f, FontStyle.Bold);
            _timeLabel.Text = "00:00:00.00 / 00:00:00.00";

            _dataLabel = new Label();
            _dataLabel.Dock = DockStyle.Fill;
            _dataLabel.TextAlign = ContentAlignment.MiddleLeft;
            _dataLabel.AutoEllipsis = true;
            _dataLabel.ForeColor = UiPalette.Text;
            _dataLabel.Font = new Font("Segoe UI", 8.6f);
            _dataLabel.Text = "Load a log with GPS data to use interactive replay.";

            var infoPanel = new Panel();
            infoPanel.Dock = DockStyle.Fill;
            infoPanel.Controls.Add(_dataLabel);
            infoPanel.Controls.Add(_timeLabel);
            bottom.Controls.Add(infoPanel);
            bottom.Controls.Add(_timeline);

            Controls.Add(_map);
            Controls.Add(bottom);
            Controls.Add(toolbarHost);

            _playTimer = new Timer();
            _playTimer.Interval = 40;
            _playTimer.Tick += PlayTimer_Tick;
        }

        public LogAnalysisResult Result
        {
            get { return _result; }
            set
            {
                StopPlayback();
                _result = value;
                _rangeStartMs = null;
                _rangeEndMs = null;
                _rangeOnlyCheck.Checked = false;
                _map.Result = value;
                PopulateGpsSelector();
                _timeline.Enabled = value != null && value.DurationSeconds > 0;
                if (value == null)
                {
                    _currentTimeMs = 0;
                    _timeline.Value = 0;
                    _timeLabel.Text = "00:00:00.00 / 00:00:00.00";
                    _dataLabel.Text = "Load a log with GPS data to use interactive replay.";
                    UpdateRangeUi(false);
                    return;
                }

                UpdateRangeUi(false);
                _map.FitTrack();
                SetTime(value.FirstTimeMs, true);
            }
        }

        public void SetTime(double timeMs, bool raiseEvent)
        {
            if (_result == null)
                return;

            double min, max;
            GetActiveBounds(out min, out max);
            _currentTimeMs = Math.Max(min, Math.Min(max, timeMs));

            _updatingTimeline = true;
            try
            {
                double fraction = max <= min ? 0 : (_currentTimeMs - min) / (max - min);
                _timeline.Value = Math.Max(_timeline.Minimum, Math.Min(_timeline.Maximum,
                    (int)Math.Round(fraction * _timeline.Maximum)));
            }
            finally
            {
                _updatingTimeline = false;
            }

            _map.CurrentTimeMs = _currentTimeMs;
            if (_followCheck.Checked)
                _map.CenterOnTime(_currentTimeMs);
            _map.Invalidate();
            UpdateStatus();

            if (raiseEvent && TimeChanged != null)
                TimeChanged(this, new MapTimeChangedEventArgs(_currentTimeMs));
        }

        public void PausePlayback()
        {
            StopPlayback();
        }

        private static Button MakeButton(string text, EventHandler handler, int width)
        {
            var button = new Button();
            button.Text = text;
            button.AutoSize = false;
            button.MinimumSize = new Size(width, 32);
            button.Size = new Size(width, 32);
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.UseCompatibleTextRendering = false;
            button.AutoEllipsis = false;
            button.Click += handler;
            UiPalette.StyleButton(button, text == "Play");
            button.Padding = new Padding(4, 0, 4, 0);
            button.Width = width;
            button.Height = 32;
            return button;
        }

        private static Label MakeLabel(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                ForeColor = UiPalette.Text,
                Font = new Font("Segoe UI Semibold", 8.8f, FontStyle.Bold),
                Padding = new Padding(8, 7, 2, 0),
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private static CheckBox MakeCheckBox(string text, bool isChecked)
        {
            var check = new CheckBox();
            check.Text = text;
            check.AutoSize = true;
            check.Checked = isChecked;
            check.Padding = new Padding(7, 5, 0, 0);
            check.ForeColor = UiPalette.Text;
            UiPalette.StyleCheckBox(check);
            return check;
        }

        private void PopulateGpsSelector()
        {
            _updatingGpsSelector = true;
            try
            {
                _gpsSelector.Items.Clear();
                if (_result == null || _result.Gps == null || _result.Gps.Count == 0)
                {
                    _gpsSelector.Items.Add(new GpsTrackOption { Label = "No GPS track", Key = null, ShowAll = false });
                    _gpsSelector.SelectedIndex = 0;
                    return;
                }

                List<KeyValuePair<string, List<GpsSample>>> tracks = _result.Gps
                    .Where(p => p.Value != null && p.Value.Any(g => IsValidCoordinate(g.Lat, g.Lng)))
                    .OrderBy(p => SensorAssessmentBuilder.InferGpsOrdinal(p.Key)).ThenBy(p => p.Key).ToList();
                if (tracks.Count == 0)
                {
                    _gpsSelector.Items.Add(new GpsTrackOption { Label = "No valid GPS coordinates", Key = null, ShowAll = false });
                    _gpsSelector.SelectedIndex = 0;
                    return;
                }

                string primary = GpsDisplayHelper.SelectPrimaryKey(_result);
                if (tracks.Count > 1)
                    _gpsSelector.Items.Add(new GpsTrackOption { Label = "All GPS receivers", ShowAll = true });
                if (!string.IsNullOrEmpty(primary))
                {
                    KeyValuePair<string, List<GpsSample>> p = tracks.FirstOrDefault(x => string.Equals(x.Key, primary, StringComparison.OrdinalIgnoreCase));
                    if (!string.IsNullOrEmpty(p.Key))
                        _gpsSelector.Items.Add(new GpsTrackOption { Key = p.Key, Label = "Primary • " + BuildGpsOptionLabel(p.Key, p.Value), ShowAll = false });
                }
                foreach (KeyValuePair<string, List<GpsSample>> pair in tracks)
                {
                    // The selected primary is already exposed with an explicit Primary label above.
                    // Avoid showing the same receiver twice in the selector.
                    if (!string.IsNullOrEmpty(primary) && string.Equals(pair.Key, primary, StringComparison.OrdinalIgnoreCase))
                        continue;
                    _gpsSelector.Items.Add(new GpsTrackOption { Key = pair.Key, Label = BuildGpsOptionLabel(pair.Key, pair.Value), ShowAll = false });
                }

                int desired = 0;
                for (int i = 0; i < _gpsSelector.Items.Count; i++)
                {
                    GpsTrackOption option = _gpsSelector.Items[i] as GpsTrackOption;
                    if (option != null && !option.ShowAll && string.Equals(option.Key, primary, StringComparison.OrdinalIgnoreCase))
                    {
                        desired = i;
                        break;
                    }
                }
                _gpsSelector.SelectedIndex = desired;
            }
            finally
            {
                _updatingGpsSelector = false;
            }
            ApplyGpsSelection();
        }

        private string BuildGpsOptionLabel(string key, List<GpsSample> samples)
        {
            int ordinal = SensorAssessmentBuilder.InferGpsOrdinal(key);
            string type = GpsDisplayHelper.GetGpsTypeName(_result, ordinal);
            bool used = samples != null && samples.Any(x => x.Used.HasValue && x.Used.Value > 0.5);
            return "GPS" + ordinal.ToString(CultureInfo.InvariantCulture) + " • " + type + " • " +
                   (samples == null ? 0 : samples.Count).ToString("N0", CultureInfo.InvariantCulture) + " samples" + (used ? " • in use" : string.Empty);
        }

        private void GpsSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_updatingGpsSelector) return;
            ApplyGpsSelection();
            _map.FitTrack();
            UpdateStatus();
        }

        private void ApplyGpsSelection()
        {
            GpsTrackOption option = _gpsSelector.SelectedItem as GpsTrackOption;
            _map.SetGpsSelection(option == null ? null : option.Key, option != null && option.ShowAll);
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            if (_playing)
            {
                StopPlayback();
                return;
            }
            _direction = 1;
            StartPlayback();
        }

        private void ReverseButton_Click(object sender, EventArgs e)
        {
            _direction = -1;
            StartPlayback();
        }

        private void StepBackButton_Click(object sender, EventArgs e)
        {
            StopPlayback();
            SetTime(_currentTimeMs - 1000.0, true);
        }

        private void StepForwardButton_Click(object sender, EventArgs e)
        {
            StopPlayback();
            SetTime(_currentTimeMs + 1000.0, true);
        }

        private void FitButton_Click(object sender, EventArgs e)
        {
            _followCheck.Checked = false;
            _map.FitTrack();
        }

        private void RangeStartButton_Click(object sender, EventArgs e)
        {
            if (_result == null) return;
            _rangeStartMs = _currentTimeMs;
            NormalizeSelectedRange();
            UpdateRangeUi(true);
        }

        private void RangeEndButton_Click(object sender, EventArgs e)
        {
            if (_result == null) return;
            _rangeEndMs = _currentTimeMs;
            NormalizeSelectedRange();
            UpdateRangeUi(true);
        }

        private void ClearRangeButton_Click(object sender, EventArgs e)
        {
            _rangeStartMs = null;
            _rangeEndMs = null;
            _rangeOnlyCheck.Checked = false;
            UpdateRangeUi(true);
        }

        private void RangeOnlyCheck_CheckedChanged(object sender, EventArgs e)
        {
            _map.ShowRangeOnly = _rangeOnlyCheck.Checked && HasCompleteRange();
            _map.RefreshRangeFilter();
            if (_result != null)
                SetTime(_currentTimeMs, true);
            _map.FitTrack();
        }

        private void NormalizeSelectedRange()
        {
            if (!_rangeStartMs.HasValue || !_rangeEndMs.HasValue) return;
            if (_rangeEndMs.Value < _rangeStartMs.Value)
            {
                double temp = _rangeStartMs.Value;
                _rangeStartMs = _rangeEndMs.Value;
                _rangeEndMs = temp;
            }
        }

        private bool HasCompleteRange()
        {
            return _rangeStartMs.HasValue && _rangeEndMs.HasValue && _rangeEndMs.Value > _rangeStartMs.Value;
        }

        private void UpdateRangeUi(bool refit)
        {
            _map.RangeStartMs = _rangeStartMs;
            _map.RangeEndMs = _rangeEndMs;
            bool complete = HasCompleteRange();
            _rangeOnlyCheck.Enabled = complete;
            if (!complete && _rangeOnlyCheck.Checked)
                _rangeOnlyCheck.Checked = false;
            _map.ShowRangeOnly = complete && _rangeOnlyCheck.Checked;
            _map.RefreshRangeFilter();

            if (_result == null)
                _rangeLabel.Text = "Selected range: full log";
            else if (_rangeStartMs.HasValue && _rangeEndMs.HasValue)
                _rangeLabel.Text = "Range: " + FormatRelative(_rangeStartMs.Value) + " → " + FormatRelative(_rangeEndMs.Value) +
                    "  (" + ((_rangeEndMs.Value - _rangeStartMs.Value) / 1000.0).ToString("0.00", CultureInfo.InvariantCulture) + " s)";
            else if (_rangeStartMs.HasValue)
                _rangeLabel.Text = "Range start: " + FormatRelative(_rangeStartMs.Value) + " • set end";
            else if (_rangeEndMs.HasValue)
                _rangeLabel.Text = "Range end: " + FormatRelative(_rangeEndMs.Value) + " • set start";
            else
                _rangeLabel.Text = "Selected range: full log";

            if (_result != null)
                SetTime(_currentTimeMs <= 0 ? _result.FirstTimeMs : _currentTimeMs, false);
            if (refit) _map.FitTrack(); else _map.Invalidate();
        }

        private void Timeline_Scroll(object sender, EventArgs e)
        {
            if (_updatingTimeline || _result == null)
                return;
            StopPlayback();
            double min, max;
            GetActiveBounds(out min, out max);
            double fraction = (double)_timeline.Value / Math.Max(1, _timeline.Maximum);
            SetTime(min + fraction * (max - min), true);
        }

        private void Map_TimeSelected(object sender, MapTimeChangedEventArgs e)
        {
            StopPlayback();
            SetTime(e.TimeMs, true);
        }

        private void StartPlayback()
        {
            if (_result == null)
                return;
            double min, max;
            GetActiveBounds(out min, out max);
            if (_direction > 0 && _currentTimeMs >= max)
                SetTime(min, true);
            else if (_direction < 0 && _currentTimeMs <= min)
                SetTime(max, true);

            _playing = true;
            _playButton.Text = "Pause";
            _lastTickUtc = DateTime.UtcNow;
            _playTimer.Start();
        }

        private void StopPlayback()
        {
            _playing = false;
            _playTimer.Stop();
            _playButton.Text = "Play";
        }

        private void PlayTimer_Tick(object sender, EventArgs e)
        {
            if (!_playing || _result == null)
                return;
            DateTime now = DateTime.UtcNow;
            double elapsedMs = Math.Max(1, (now - _lastTickUtc).TotalMilliseconds);
            _lastTickUtc = now;
            double next = _currentTimeMs + elapsedMs * GetPlaybackSpeed() * _direction;
            double min, max;
            GetActiveBounds(out min, out max);
            if (next >= max)
            {
                SetTime(max, true);
                StopPlayback();
            }
            else if (next <= min)
            {
                SetTime(min, true);
                StopPlayback();
            }
            else
                SetTime(next, true);
        }

        private void GetActiveBounds(out double min, out double max)
        {
            if (_result == null)
            {
                min = 0; max = 0; return;
            }
            min = _result.FirstTimeMs;
            max = Math.Max(min, _result.LastTimeMs);
            if (_rangeOnlyCheck.Checked && HasCompleteRange())
            {
                min = Math.Max(min, _rangeStartMs.Value);
                max = Math.Min(max, _rangeEndMs.Value);
            }
        }

        private double GetPlaybackSpeed()
        {
            string text = Convert.ToString(_speedSelector.SelectedItem, CultureInfo.InvariantCulture) ?? "1x";
            double speed;
            if (double.TryParse(text.TrimEnd('x'), NumberStyles.Float, CultureInfo.InvariantCulture, out speed))
                return speed;
            return 1.0;
        }

        private void UpdateStatus()
        {
            if (_result == null)
                return;
            double elapsedSeconds = Math.Max(0, (_currentTimeMs - _result.FirstTimeMs) / 1000.0);
            _timeLabel.Text = FormatClock(elapsedSeconds) + " / " + FormatClock(_result.DurationSeconds);

            GpsSample gps = _map.FindNearestByTime(_currentTimeMs);
            if (gps == null)
            {
                _dataLabel.Text = "No GPS sample near this time for the selected receiver.";
                return;
            }

            string mode = GetModeAtTime(_result, _currentTimeMs);
            string altitude = gps.Altitude.HasValue ? gps.Altitude.Value.ToString("0.0", CultureInfo.InvariantCulture) + " m" : "n/a";
            string speed = gps.Speed.HasValue ? gps.Speed.Value.ToString("0.0", CultureInfo.InvariantCulture) + " m/s" : "n/a";
            string sats = gps.Sats.HasValue ? gps.Sats.Value.ToString("0", CultureInfo.InvariantCulture) : "n/a";
            string hdop = gps.Hdop.HasValue ? gps.Hdop.Value.ToString("0.00", CultureInfo.InvariantCulture) : "n/a";
            _dataLabel.Text = string.Format(CultureInfo.InvariantCulture,
                "{0}   Mode: {1}   Phase: {2}   Alt: {3}   Speed: {4}   Sats: {5}   HDOP: {6}   Lat/Lon: {7:0.000000}, {8:0.000000}",
                _map.ActiveTrackLabel, mode, gps.Phase, altitude, speed, sats, hdop, gps.Lat ?? 0, gps.Lng ?? 0);
        }

        private string FormatRelative(double timeMs)
        {
            double seconds = _result == null ? 0 : Math.Max(0, (timeMs - _result.FirstTimeMs) / 1000.0);
            return FormatClock(seconds);
        }

        private static bool IsValidCoordinate(double? lat, double? lng)
        {
            return lat.HasValue && lng.HasValue && Math.Abs(lat.Value) <= 90 && Math.Abs(lng.Value) <= 180 &&
                   !(Math.Abs(lat.Value) < 0.000001 && Math.Abs(lng.Value) < 0.000001);
        }

        private static string GetModeAtTime(LogAnalysisResult result, double timeMs)
        {
            string mode = "Unknown";
            foreach (var item in result.Timeline.Where(t => t.Type == "Mode" && t.TimeMs <= timeMs).OrderBy(t => t.TimeMs))
                mode = item.Text.Replace("Flight mode changed to ", string.Empty);
            return mode;
        }

        private static string FormatClock(double seconds)
        {
            if (seconds < 0) seconds = 0;
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            return string.Format(CultureInfo.InvariantCulture, "{0:00}:{1:00}:{2:00}.{3:00}",
                (int)time.TotalHours, time.Minutes, time.Seconds, time.Milliseconds / 10);
        }
    }

    internal sealed class InteractiveFlightMapControl : Control
    {
        private LogAnalysisResult _result;
        private Dictionary<string, List<GpsSample>> _tracks = new Dictionary<string, List<GpsSample>>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, List<GpsSample>> _displayTracks = new Dictionary<string, List<GpsSample>>(StringComparer.OrdinalIgnoreCase);
        private List<GpsSample> _track = new List<GpsSample>();
        private List<GpsSample> _displayTrack = new List<GpsSample>();
        private string _selectedGpsKey;
        private bool _showAllGps;
        private double _originLat;
        private double _originLng;
        private double _fitSpanX = 100;
        private double _fitSpanY = 100;
        private double _centerX;
        private double _centerY;
        private double _zoom = 1.0;
        private bool _dragging;
        private bool _dragMoved;
        private Point _mouseDown;
        private Point _lastMouse;
        private GpsSample _hoverSample;
        private Finding _hoverFinding;
        private Point _hoverPoint;
        private readonly ToolTip _toolTip;

        public event EventHandler<MapTimeChangedEventArgs> TimeSelected;
        public double CurrentTimeMs { get; set; }
        public bool ShowEvents { get; set; }
        public bool ShowFuturePath { get; set; }
        public bool ShowWaypoints { get; set; }
        public bool ShowModes { get; set; }
        public bool ShowReferences { get; set; }
        public bool ShowRangeOnly { get; set; }
        public double? RangeStartMs { get; set; }
        public double? RangeEndMs { get; set; }
        public string ActiveTrackLabel
        {
            get
            {
                if (_result == null || string.IsNullOrEmpty(_selectedGpsKey)) return "GPS: n/a";
                int ordinal = SensorAssessmentBuilder.InferGpsOrdinal(_selectedGpsKey);
                return "GPS" + ordinal.ToString(CultureInfo.InvariantCulture) + " " + GpsDisplayHelper.GetGpsTypeName(_result, ordinal);
            }
        }

        public InteractiveFlightMapControl()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            BackColor = UiPalette.AppBackground;
            ForeColor = UiPalette.Text;
            Font = new Font("Segoe UI", 8.6f);
            TabStop = true;
            ShowEvents = true;
            ShowFuturePath = true;
            ShowWaypoints = true;
            ShowModes = true;
            ShowReferences = true;
            _toolTip = new ToolTip();
        }

        public LogAnalysisResult Result
        {
            get { return _result; }
            set
            {
                _result = value;
                LoadTracks();
                FitTrack();
            }
        }

        public void SetGpsSelection(string key, bool showAll)
        {
            _selectedGpsKey = key;
            _showAllGps = showAll;
            ApplyGpsSelection();
            Invalidate();
        }

        public void RefreshRangeFilter()
        {
            ApplyGpsSelection();
            Invalidate();
        }

        public void FitTrack()
        {
            List<GpsSample> samples = GetFitSamples();
            if (samples.Count < 2)
            {
                _zoom = 1.0;
                _centerX = _centerY = 0;
                Invalidate();
                return;
            }

            List<PointD> points = samples.Select(ToWorld).ToList();
            double minX = points.Min(p => p.X);
            double maxX = points.Max(p => p.X);
            double minY = points.Min(p => p.Y);
            double maxY = points.Max(p => p.Y);
            _centerX = (minX + maxX) / 2.0;
            _centerY = (minY + maxY) / 2.0;
            _fitSpanX = Math.Max(20, (maxX - minX) * 1.14);
            _fitSpanY = Math.Max(20, (maxY - minY) * 1.14);
            _zoom = 1.0;
            Invalidate();
        }

        public void CenterOnTime(double timeMs)
        {
            GpsSample sample = FindNearestByTime(timeMs);
            if (sample == null) return;
            PointD world = ToWorld(sample);
            _centerX = world.X;
            _centerY = world.Y;
            Invalidate();
        }

        public GpsSample FindNearestByTime(double timeMs)
        {
            return FindNearestInTrack(_track, timeMs);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(UiPalette.AppBackground);
            Rectangle plot = new Rectangle(10, 10, Math.Max(20, ClientSize.Width - 20), Math.Max(20, ClientSize.Height - 20));
            using (var surface = new SolidBrush(UiPalette.PlotSurface)) g.FillRectangle(surface, plot);
            DrawGrid(g, plot);

            if (_result == null)
            {
                DrawCentered(g, "No log selected", plot);
                return;
            }
            if (_track.Count < 2)
            {
                DrawCentered(g, "No valid GPS track available for the selected receiver", plot);
                return;
            }

            DrawTracks(g, plot);
            DrawSelectedRange(g, plot);
            DrawStartEnd(g, plot);
            if (ShowReferences) DrawReferences(g, plot);
            if (ShowWaypoints) DrawWaypoints(g, plot);
            if (ShowModes) DrawModes(g, plot);
            if (ShowEvents) DrawFindings(g, plot);
            DrawVehicle(g, plot);
            DrawGpsLegend(g, plot);
            DrawCompassAndScale(g, plot);
            DrawInstructions(g, plot);
            DrawHoverBox(g, plot);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
            if (e.Button == MouseButtons.Left)
            {
                _dragging = true;
                _dragMoved = false;
                _mouseDown = e.Location;
                _lastMouse = e.Location;
                Cursor = Cursors.Hand;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_dragging)
            {
                int dx = e.X - _lastMouse.X;
                int dy = e.Y - _lastMouse.Y;
                if (Math.Abs(e.X - _mouseDown.X) + Math.Abs(e.Y - _mouseDown.Y) > 4) _dragMoved = true;
                Rectangle plot = GetPlotRectangle();
                ViewSpans spans = GetViewSpans(plot);
                _centerX -= dx * spans.X / Math.Max(1, plot.Width);
                _centerY += dy * spans.Y / Math.Max(1, plot.Height);
                _lastMouse = e.Location;
                Invalidate();
                return;
            }
            UpdateHover(e.Location);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button != MouseButtons.Left) return;
            _dragging = false;
            Cursor = Cursors.Default;
            if (!_dragMoved) SelectNearestTime(e.Location);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            Rectangle plot = GetPlotRectangle();
            if (!plot.Contains(e.Location) || _track.Count == 0) return;
            PointD before = ScreenToWorld(e.Location, plot);
            double factor = e.Delta > 0 ? 1.25 : 0.8;
            _zoom = Math.Max(0.35, Math.Min(200.0, _zoom * factor));
            PointD after = ScreenToWorld(e.Location, plot);
            _centerX += before.X - after.X;
            _centerY += before.Y - after.Y;
            Invalidate();
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            FitTrack();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Add || e.KeyCode == Keys.Oemplus)
            {
                _zoom = Math.Min(200, _zoom * 1.25); Invalidate(); e.Handled = true;
            }
            else if (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus)
            {
                _zoom = Math.Max(0.35, _zoom * 0.8); Invalidate(); e.Handled = true;
            }
            else if (e.KeyCode == Keys.Home)
            {
                FitTrack(); e.Handled = true;
            }
        }

        private void LoadTracks()
        {
            _tracks = new Dictionary<string, List<GpsSample>>(StringComparer.OrdinalIgnoreCase);
            _displayTracks = new Dictionary<string, List<GpsSample>>(StringComparer.OrdinalIgnoreCase);
            _track = new List<GpsSample>();
            _displayTrack = new List<GpsSample>();
            if (_result == null || _result.Gps == null) return;

            foreach (KeyValuePair<string, List<GpsSample>> pair in _result.Gps)
            {
                List<GpsSample> valid = pair.Value.Where(s => IsValidCoordinate(s.Lat, s.Lng)).OrderBy(s => s.TimeMs).ToList();
                if (valid.Count > 0) _tracks[pair.Key] = valid;
            }
            if (_tracks.Count == 0) return;
            if (string.IsNullOrEmpty(_selectedGpsKey) || !_tracks.ContainsKey(_selectedGpsKey))
                _selectedGpsKey = GpsDisplayHelper.SelectPrimaryKey(_result) ?? _tracks.Keys.First();
            ApplyGpsSelection();
            CurrentTimeMs = _result.FirstTimeMs;
        }

        private void ApplyGpsSelection()
        {
            if (_tracks.Count == 0)
            {
                _track = new List<GpsSample>();
                _displayTrack = new List<GpsSample>();
                return;
            }
            if (string.IsNullOrEmpty(_selectedGpsKey) || !_tracks.ContainsKey(_selectedGpsKey))
                _selectedGpsKey = GpsDisplayHelper.SelectPrimaryKey(_result) ?? _tracks.Keys.First();

            _track = _tracks[_selectedGpsKey];
            _displayTrack = Downsample(FilterForRange(_track), 6000);
            _displayTracks.Clear();
            foreach (KeyValuePair<string, List<GpsSample>> pair in _tracks)
                _displayTracks[pair.Key] = Downsample(FilterForRange(pair.Value), 4000);

            IEnumerable<GpsSample> originSource = _showAllGps ? _tracks.Values.SelectMany(x => x) : _track;
            List<GpsSample> originSamples = originSource.Where(x => IsValidCoordinate(x.Lat, x.Lng)).ToList();
            if (originSamples.Count > 0)
            {
                _originLat = originSamples.Average(s => s.Lat.Value);
                _originLng = originSamples.Average(s => s.Lng.Value);
            }
            _hoverSample = null;
            _hoverFinding = null;
        }

        private List<GpsSample> FilterForRange(List<GpsSample> source)
        {
            if (!ShowRangeOnly || !HasRange()) return source;
            return source.Where(x => x.TimeMs >= RangeStartMs.Value && x.TimeMs <= RangeEndMs.Value).ToList();
        }

        private bool HasRange()
        {
            return RangeStartMs.HasValue && RangeEndMs.HasValue && RangeEndMs.Value > RangeStartMs.Value;
        }

        private List<GpsSample> GetFitSamples()
        {
            IEnumerable<GpsSample> samples = _showAllGps ? _tracks.Values.SelectMany(x => x) : _track;
            if (ShowRangeOnly && HasRange()) samples = samples.Where(x => x.TimeMs >= RangeStartMs.Value && x.TimeMs <= RangeEndMs.Value);
            return samples.Where(x => IsValidCoordinate(x.Lat, x.Lng)).ToList();
        }

        private static bool IsValidCoordinate(double? lat, double? lng)
        {
            return lat.HasValue && lng.HasValue && Math.Abs(lat.Value) <= 90 && Math.Abs(lng.Value) <= 180 &&
                   !(Math.Abs(lat.Value) < 0.000001 && Math.Abs(lng.Value) < 0.000001);
        }

        private static List<GpsSample> Downsample(List<GpsSample> source, int maximum)
        {
            if (source == null || source.Count <= maximum) return source == null ? new List<GpsSample>() : new List<GpsSample>(source);
            var output = new List<GpsSample>(maximum);
            double step = (double)(source.Count - 1) / (maximum - 1);
            for (int i = 0; i < maximum; i++) output.Add(source[Math.Min(source.Count - 1, (int)Math.Round(i * step))]);
            return output;
        }

        private void DrawGrid(Graphics g, Rectangle plot)
        {
            ViewSpans spans = GetViewSpans(plot);
            double target = Math.Max(spans.X / 8.0, spans.Y / 8.0);
            double spacing = NiceSpacing(target);
            double left = _centerX - spans.X / 2.0;
            double right = _centerX + spans.X / 2.0;
            double bottom = _centerY - spans.Y / 2.0;
            double top = _centerY + spans.Y / 2.0;
            using (var minor = new Pen(UiPalette.Grid, 1f))
            using (var major = new Pen(UiPalette.GridMajor, 1f))
            {
                double firstX = Math.Floor(left / spacing) * spacing;
                for (double x = firstX; x <= right; x += spacing)
                {
                    PointF p1 = WorldToScreen(new PointD(x, bottom), plot); PointF p2 = WorldToScreen(new PointD(x, top), plot);
                    g.DrawLine(Math.Abs(x) < spacing * 0.1 ? major : minor, p1, p2);
                }
                double firstY = Math.Floor(bottom / spacing) * spacing;
                for (double y = firstY; y <= top; y += spacing)
                {
                    PointF p1 = WorldToScreen(new PointD(left, y), plot); PointF p2 = WorldToScreen(new PointD(right, y), plot);
                    g.DrawLine(Math.Abs(y) < spacing * 0.1 ? major : minor, p1, p2);
                }
            }
            using (var border = new Pen(UiPalette.Border, 1.2f)) g.DrawRectangle(border, plot);
        }

        private void DrawTracks(Graphics g, Rectangle plot)
        {
            if (_showAllGps)
            {
                foreach (KeyValuePair<string, List<GpsSample>> pair in _displayTracks.OrderBy(p => SensorAssessmentBuilder.InferGpsOrdinal(p.Key)))
                    DrawOneTrack(g, plot, pair.Value, GpsColor(SensorAssessmentBuilder.InferGpsOrdinal(pair.Key)), string.Equals(pair.Key, _selectedGpsKey, StringComparison.OrdinalIgnoreCase));
            }
            else
                DrawOneTrack(g, plot, _displayTrack, Color.Empty, true);
        }

        private void DrawOneTrack(Graphics g, Rectangle plot, List<GpsSample> points, Color receiverColor, bool active)
        {
            if (points == null || points.Count < 2) return;
            for (int i = 1; i < points.Count; i++)
            {
                GpsSample a = points[i - 1]; GpsSample b = points[i];
                bool past = b.TimeMs <= CurrentTimeMs;
                if (!past && !ShowFuturePath) continue;
                Color color;
                if (_showAllGps)
                    color = past ? receiverColor : Color.FromArgb(90, receiverColor);
                else
                    color = past ? PhaseColor(b.Phase) : Color.FromArgb(105, 118, 135);
                float width = active ? (past ? 3.0f : 1.5f) : (past ? 1.8f : 1.0f);
                using (var pen = new Pen(color, width)) g.DrawLine(pen, WorldToScreen(ToWorld(a), plot), WorldToScreen(ToWorld(b), plot));
            }
        }

        private void DrawSelectedRange(Graphics g, Rectangle plot)
        {
            if (!HasRange() || _track.Count < 2) return;
            List<GpsSample> segment = _track.Where(x => x.TimeMs >= RangeStartMs.Value && x.TimeMs <= RangeEndMs.Value).ToList();
            if (segment.Count >= 2)
            {
                using (var pen = new Pen(UiPalette.Replay, 4.2f))
                    for (int i = 1; i < segment.Count; i++) g.DrawLine(pen, WorldToScreen(ToWorld(segment[i - 1]), plot), WorldToScreen(ToWorld(segment[i]), plot));
            }
            GpsSample start = FindNearestByTime(RangeStartMs.Value); GpsSample end = FindNearestByTime(RangeEndMs.Value);
            if (start != null)
            {
                PointF p = WorldToScreen(ToWorld(start), plot);
                using (var b = new SolidBrush(UiPalette.Advisory)) g.FillEllipse(b, p.X - 7, p.Y - 7, 14, 14);
                using (var b = new SolidBrush(UiPalette.Advisory)) g.DrawString("Range start", Font, b, p.X + 8, p.Y - 18);
            }
            if (end != null)
            {
                PointF p = WorldToScreen(ToWorld(end), plot);
                using (var b = new SolidBrush(UiPalette.Replay)) g.FillEllipse(b, p.X - 7, p.Y - 7, 14, 14);
                using (var b = new SolidBrush(UiPalette.Replay)) g.DrawString("Range end", Font, b, p.X + 8, p.Y - 18);
            }
        }

        private void DrawStartEnd(Graphics g, Rectangle plot)
        {
            List<GpsSample> view = FilterForRange(_track);
            if (view.Count == 0) return;
            PointF start = WorldToScreen(ToWorld(view[0]), plot); PointF end = WorldToScreen(ToWorld(view[view.Count - 1]), plot);
            using (var brush = new SolidBrush(UiPalette.Success)) g.FillEllipse(brush, start.X - 5, start.Y - 5, 10, 10);
            using (var brush = new SolidBrush(UiPalette.Critical)) g.FillEllipse(brush, end.X - 5, end.Y - 5, 10, 10);
            if (!HasRange())
            {
                using (var b = new SolidBrush(UiPalette.Success)) g.DrawString("Start", Font, b, start.X + 7, start.Y - 17);
                using (var b = new SolidBrush(UiPalette.Critical)) g.DrawString("End", Font, b, end.X + 7, end.Y - 17);
            }
        }

        private void DrawFindings(Graphics g, Rectangle plot)
        {
            foreach (Finding finding in _result.Findings.Where(f => (f.Severity == "Critical" || f.Severity == "Warning") && InRange(f.TimeMs)).Take(30))
            {
                GpsSample gps = FindNearestByTime(finding.TimeMs); if (gps == null) continue;
                PointF p = WorldToScreen(ToWorld(gps), plot);
                Color color = finding.Severity == "Critical" ? UiPalette.Critical : UiPalette.Warning;
                PointF[] diamond = { new PointF(p.X, p.Y - 7), new PointF(p.X + 7, p.Y), new PointF(p.X, p.Y + 7), new PointF(p.X - 7, p.Y) };
                using (var brush = new SolidBrush(color)) g.FillPolygon(brush, diamond);
                g.DrawPolygon(Pens.White, diamond);
            }
        }

        private void DrawModes(Graphics g, Rectangle plot)
        {
            if (_result == null) return;
            int drawn = 0;
            foreach (TimelineEvent mode in _result.Timeline.Where(x => x.Type == "Mode" && InRange(x.TimeMs)).OrderBy(x => x.TimeMs))
            {
                if (drawn++ >= 24) break;
                GpsSample gps = FindNearestByTime(mode.TimeMs); if (gps == null) continue;
                PointF p = WorldToScreen(ToWorld(gps), plot);
                PointF[] tri = { new PointF(p.X, p.Y - 9), new PointF(p.X + 7, p.Y + 5), new PointF(p.X - 7, p.Y + 5) };
                using (var b = new SolidBrush(UiPalette.Advisory)) g.FillPolygon(b, tri);
                string label = mode.Text.Replace("Flight mode changed to ", string.Empty);
                using (var b = new SolidBrush(UiPalette.Advisory)) g.DrawString(label, Font, b, p.X + 9, p.Y - 10);
            }
        }

        private void DrawWaypoints(Graphics g, Rectangle plot)
        {
            if (_result == null || _result.MissionCommands == null) return;
            List<MissionCommandRecord> source = _result.MissionCommands.Where(x => !x.Executed && IsValidCoordinate(x.Lat, x.Lng)).ToList();
            if (source.Count == 0) source = _result.MissionCommands.Where(x => IsValidCoordinate(x.Lat, x.Lng)).ToList();
            foreach (MissionCommandRecord wp in source.GroupBy(x => x.Sequence).Select(gp => gp.Last()).OrderBy(x => x.Sequence).Take(80))
            {
                PointF p = WorldToScreen(ToWorld(wp.Lat.Value, wp.Lng.Value), plot);
                using (var b = new SolidBrush(UiPalette.Warning)) g.FillEllipse(b, p.X - 8, p.Y - 8, 16, 16);
                using (var b = new SolidBrush(UiPalette.Navy))
                using (var f = new Font("Segoe UI", 7.2f, FontStyle.Bold))
                    TextRenderer.DrawText(g, wp.Sequence.ToString(CultureInfo.InvariantCulture), f, new Rectangle((int)p.X - 8, (int)p.Y - 8, 16, 16), UiPalette.Navy, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                using (var b = new SolidBrush(UiPalette.Warning)) g.DrawString(wp.CommandName, Font, b, p.X + 10, p.Y - 8);
            }
        }

        private void DrawReferences(Graphics g, Rectangle plot)
        {
            if (_result == null || _result.ReferencePoints == null) return;
            foreach (ReferencePointRecord r in _result.ReferencePoints.Where(x => IsValidCoordinate(x.Lat, x.Lng)).GroupBy(x => x.Type).Select(gp => gp.Last()).Take(12))
            {
                PointF p = WorldToScreen(ToWorld(r.Lat, r.Lng), plot);
                using (var pen = new Pen(UiPalette.Success, 2f))
                {
                    g.DrawRectangle(pen, p.X - 7, p.Y - 7, 14, 14);
                    g.DrawLine(pen, p.X - 10, p.Y, p.X + 10, p.Y);
                    g.DrawLine(pen, p.X, p.Y - 10, p.X, p.Y + 10);
                }
                using (var b = new SolidBrush(UiPalette.Success)) g.DrawString(r.Label, Font, b, p.X + 10, p.Y + 4);
            }
        }

        private void DrawVehicle(Graphics g, Rectangle plot)
        {
            GpsSample current = FindNearestByTime(CurrentTimeMs); if (current == null) return;
            PointF p = WorldToScreen(ToWorld(current), plot);
            double course = current.Course ?? EstimateCourse(current);
            double radians = course * Math.PI / 180.0;
            PointF forward = new PointF((float)Math.Sin(radians), (float)-Math.Cos(radians));
            PointF side = new PointF(-forward.Y, forward.X);
            float length = 13f, halfWidth = 7f;
            PointF nose = new PointF(p.X + forward.X * length, p.Y + forward.Y * length);
            PointF left = new PointF(p.X - forward.X * 7 + side.X * halfWidth, p.Y - forward.Y * 7 + side.Y * halfWidth);
            PointF right = new PointF(p.X - forward.X * 7 - side.X * halfWidth, p.Y - forward.Y * 7 - side.Y * halfWidth);
            using (var brush = new SolidBrush(UiPalette.Replay)) g.FillPolygon(brush, new[] { nose, left, right });
            using (var pen = new Pen(Color.White, 2)) g.DrawPolygon(pen, new[] { nose, left, right });
            using (var pen = new Pen(UiPalette.Replay, 2)) g.DrawEllipse(pen, p.X - 17, p.Y - 17, 34, 34);
        }

        private void DrawGpsLegend(Graphics g, Rectangle plot)
        {
            if (!_showAllGps || _tracks.Count < 2) return;
            int x = plot.Right - 185, y = plot.Top + 68, row = 0;
            using (var bg = new SolidBrush(Color.FromArgb(225, UiPalette.SurfaceRaised))) g.FillRectangle(bg, x - 8, y - 6, 177, Math.Min(8, _tracks.Count) * 20 + 12);
            foreach (KeyValuePair<string, List<GpsSample>> pair in _tracks.OrderBy(p => SensorAssessmentBuilder.InferGpsOrdinal(p.Key)).Take(8))
            {
                int ordinal = SensorAssessmentBuilder.InferGpsOrdinal(pair.Key);
                Color c = GpsColor(ordinal);
                using (var pen = new Pen(c, 3)) g.DrawLine(pen, x, y + row * 20 + 8, x + 18, y + row * 20 + 8);
                using (var b = new SolidBrush(UiPalette.Text)) g.DrawString("GPS" + ordinal + " " + GpsDisplayHelper.GetGpsTypeName(_result, ordinal), Font, b, x + 24, y + row * 20);
                row++;
            }
        }

        private void DrawCompassAndScale(Graphics g, Rectangle plot)
        {
            using (var compassBrush = new SolidBrush(UiPalette.TextStrong))
            using (var compassPen = new Pen(UiPalette.TextStrong, 1.5f))
            using (var compassFont = new Font(Font, FontStyle.Bold))
            {
                g.DrawString("N", compassFont, compassBrush, plot.Right - 28, plot.Top + 8);
                g.DrawLine(compassPen, plot.Right - 22, plot.Top + 30, plot.Right - 22, plot.Top + 52);
                g.DrawLine(compassPen, plot.Right - 22, plot.Top + 30, plot.Right - 27, plot.Top + 38);
                g.DrawLine(compassPen, plot.Right - 22, plot.Top + 30, plot.Right - 17, plot.Top + 38);
            }
            ViewSpans spans = GetViewSpans(plot);
            double scaleMeters = NiceSpacing(spans.X * 0.18);
            float pixels = (float)(scaleMeters / spans.X * plot.Width), x = plot.Left + 18, y = plot.Bottom - 30;
            using (var pen = new Pen(UiPalette.TextStrong, 2))
            {
                g.DrawLine(pen, x, y, x + pixels, y); g.DrawLine(pen, x, y - 5, x, y + 5); g.DrawLine(pen, x + pixels, y - 5, x + pixels, y + 5);
            }
            string label = scaleMeters >= 1000 ? (scaleMeters / 1000.0).ToString("0.#", CultureInfo.InvariantCulture) + " km" : scaleMeters.ToString("0", CultureInfo.InvariantCulture) + " m";
            using (var b = new SolidBrush(UiPalette.TextStrong)) g.DrawString(label, Font, b, x, y - 22);
        }

        private void DrawInstructions(Graphics g, Rectangle plot)
        {
            string text = "Drag: pan  |  Wheel: zoom  |  Click route/event: seek  |  Double-click/Home: fit  |  Pink line: selected range";
            SizeF size = g.MeasureString(text, Font);
            using (var brush = new SolidBrush(Color.FromArgb(230, UiPalette.SurfaceRaised))) g.FillRectangle(brush, plot.Left + 6, plot.Top + 6, size.Width + 12, size.Height + 7);
            using (var b = new SolidBrush(UiPalette.Muted)) g.DrawString(text, Font, b, plot.Left + 12, plot.Top + 9);
        }

        private void DrawHoverBox(Graphics g, Rectangle plot)
        {
            if (_hoverSample == null) return;
            string line1 = ActiveTrackLabel + "   " + FormatRelativeTime(_hoverSample.TimeMs) + "   " + (_hoverSample.Phase ?? "Unknown phase");
            string line2 = string.Format(CultureInfo.InvariantCulture, "Lat/Lon {0:0.000000}, {1:0.000000}   Speed {2}   Alt {3}",
                _hoverSample.Lat ?? 0, _hoverSample.Lng ?? 0,
                _hoverSample.Speed.HasValue ? _hoverSample.Speed.Value.ToString("0.0", CultureInfo.InvariantCulture) + " m/s" : "n/a",
                _hoverSample.Altitude.HasValue ? _hoverSample.Altitude.Value.ToString("0.0", CultureInfo.InvariantCulture) + " m" : "n/a");
            string line3 = _hoverFinding == null ? "Click to move playback to this point." : _hoverFinding.Severity + ": " + _hoverFinding.Title;
            string[] lines = { line1, line2, line3 };
            float width = lines.Max(t => g.MeasureString(t, Font).Width) + 18, height = lines.Length * (Font.Height + 2) + 12;
            float x = Math.Min(plot.Right - width - 4, _hoverPoint.X + 14), y = Math.Min(plot.Bottom - height - 4, _hoverPoint.Y + 14);
            x = Math.Max(plot.Left + 4, x); y = Math.Max(plot.Top + 4, y);
            using (var b = new SolidBrush(UiPalette.TooltipBackground)) g.FillRectangle(b, x, y, width, height);
            using (var pen = new Pen(UiPalette.TooltipBorder)) g.DrawRectangle(pen, x, y, width, height);
            for (int i = 0; i < lines.Length; i++)
            {
                Color c = i == 2 && _hoverFinding != null ? UiPalette.Critical : UiPalette.TextStrong;
                using (var b = new SolidBrush(c)) g.DrawString(lines[i], Font, b, x + 8, y + 6 + i * (Font.Height + 2));
            }
        }

        private void UpdateHover(Point location)
        {
            Rectangle plot = GetPlotRectangle();
            if (!plot.Contains(location) || _displayTrack.Count == 0)
            {
                if (_hoverSample != null) { _hoverSample = null; _hoverFinding = null; Invalidate(); }
                return;
            }
            GpsSample nearest = FindNearestScreenSample(location, plot, 18.0);
            Finding finding = FindNearestFinding(location, plot, 12.0);
            if (!ReferenceEquals(nearest, _hoverSample) || !ReferenceEquals(finding, _hoverFinding))
            {
                _hoverSample = nearest; _hoverFinding = finding;
                if (finding != null) _hoverSample = FindNearestByTime(finding.TimeMs);
                _hoverPoint = location; Invalidate();
            }
            else _hoverPoint = location;
        }

        private void SelectNearestTime(Point location)
        {
            Rectangle plot = GetPlotRectangle();
            Finding finding = FindNearestFinding(location, plot, 15.0);
            double timeMs = 0;
            if (finding != null) timeMs = finding.TimeMs;
            else
            {
                GpsSample sample = FindNearestScreenSample(location, plot, 24.0);
                if (sample != null) timeMs = sample.TimeMs;
            }
            if (timeMs > 0 && TimeSelected != null) TimeSelected(this, new MapTimeChangedEventArgs(timeMs));
        }

        private GpsSample FindNearestScreenSample(Point location, Rectangle plot, double maximumPixels)
        {
            GpsSample best = null; double bestDistance = maximumPixels * maximumPixels;
            foreach (GpsSample sample in _displayTrack)
            {
                PointF p = WorldToScreen(ToWorld(sample), plot); double dx = p.X - location.X, dy = p.Y - location.Y, distance = dx * dx + dy * dy;
                if (distance < bestDistance) { bestDistance = distance; best = sample; }
            }
            return best;
        }

        private Finding FindNearestFinding(Point location, Rectangle plot, double maximumPixels)
        {
            if (!ShowEvents || _result == null) return null;
            Finding best = null; double bestDistance = maximumPixels * maximumPixels;
            foreach (Finding finding in _result.Findings.Where(f => (f.Severity == "Critical" || f.Severity == "Warning") && InRange(f.TimeMs)).Take(30))
            {
                GpsSample gps = FindNearestByTime(finding.TimeMs); if (gps == null) continue;
                PointF p = WorldToScreen(ToWorld(gps), plot); double dx = p.X - location.X, dy = p.Y - location.Y, distance = dx * dx + dy * dy;
                if (distance < bestDistance) { bestDistance = distance; best = finding; }
            }
            return best;
        }

        private bool InRange(double timeMs)
        {
            return !ShowRangeOnly || !HasRange() || (timeMs >= RangeStartMs.Value && timeMs <= RangeEndMs.Value);
        }

        private static GpsSample FindNearestInTrack(List<GpsSample> track, double timeMs)
        {
            if (track == null || track.Count == 0) return null;
            int low = 0, high = track.Count - 1;
            while (low <= high)
            {
                int mid = low + (high - low) / 2; double value = track[mid].TimeMs;
                if (value < timeMs) low = mid + 1; else if (value > timeMs) high = mid - 1; else return track[mid];
            }
            int a = Math.Max(0, Math.Min(track.Count - 1, low)), b = Math.Max(0, Math.Min(track.Count - 1, low - 1));
            return Math.Abs(track[a].TimeMs - timeMs) < Math.Abs(track[b].TimeMs - timeMs) ? track[a] : track[b];
        }

        private double EstimateCourse(GpsSample sample)
        {
            int index = _track.BinarySearch(sample, GpsTimeComparer.Instance); if (index < 0) index = ~index;
            int a = Math.Max(0, Math.Min(_track.Count - 1, index - 1)), b = Math.Max(0, Math.Min(_track.Count - 1, index + 1));
            PointD p1 = ToWorld(_track[a]), p2 = ToWorld(_track[b]); double dx = p2.X - p1.X, dy = p2.Y - p1.Y;
            if (Math.Abs(dx) + Math.Abs(dy) < 0.01) return 0;
            double heading = Math.Atan2(dx, dy) * 180.0 / Math.PI; if (heading < 0) heading += 360; return heading;
        }

        private static Color GpsColor(int ordinal)
        {
            if (ordinal == 1) return UiPalette.Accent;
            if (ordinal == 2) return UiPalette.Success;
            if (ordinal == 3) return UiPalette.Warning;
            return UiPalette.Advisory;
        }

        private static Color PhaseColor(string phase)
        {
            string p = (phase ?? string.Empty).ToUpperInvariant();
            if (p.Contains("TAKEOFF")) return Color.ForestGreen;
            if (p.Contains("CLIMB")) return Color.FromArgb(91, 174, 255);
            if (p.Contains("DESCENT")) return Color.FromArgb(255, 184, 92);
            if (p.Contains("LAND")) return Color.Firebrick;
            if (p.Contains("GROUND")) return Color.DimGray;
            return Color.FromArgb(99, 197, 224);
        }

        private Rectangle GetPlotRectangle() { return new Rectangle(8, 8, Math.Max(20, ClientSize.Width - 16), Math.Max(20, ClientSize.Height - 16)); }

        private ViewSpans GetViewSpans(Rectangle plot)
        {
            double spanX = Math.Max(1, _fitSpanX / Math.Max(0.01, _zoom)), spanY = Math.Max(1, _fitSpanY / Math.Max(0.01, _zoom));
            double aspect = (double)Math.Max(1, plot.Width) / Math.Max(1, plot.Height);
            if (spanX / spanY < aspect) spanX = spanY * aspect; else spanY = spanX / aspect;
            return new ViewSpans { X = spanX, Y = spanY };
        }

        private PointF WorldToScreen(PointD world, Rectangle plot)
        {
            ViewSpans spans = GetViewSpans(plot);
            float x = plot.Left + (float)((world.X - (_centerX - spans.X / 2.0)) / spans.X * plot.Width);
            float y = plot.Bottom - (float)((world.Y - (_centerY - spans.Y / 2.0)) / spans.Y * plot.Height);
            return new PointF(x, y);
        }

        private PointD ScreenToWorld(Point screen, Rectangle plot)
        {
            ViewSpans spans = GetViewSpans(plot);
            double x = (_centerX - spans.X / 2.0) + (screen.X - plot.Left) / (double)Math.Max(1, plot.Width) * spans.X;
            double y = (_centerY - spans.Y / 2.0) + (plot.Bottom - screen.Y) / (double)Math.Max(1, plot.Height) * spans.Y;
            return new PointD(x, y);
        }

        private PointD ToWorld(GpsSample sample) { return ToWorld(sample.Lat ?? _originLat, sample.Lng ?? _originLng); }
        private PointD ToWorld(double lat, double lng)
        {
            double metersPerDegreeLat = 111132.0, metersPerDegreeLng = 111320.0 * Math.Cos(_originLat * Math.PI / 180.0);
            return new PointD((lng - _originLng) * metersPerDegreeLng, (lat - _originLat) * metersPerDegreeLat);
        }

        private static double NiceSpacing(double target)
        {
            if (target <= 0) return 1;
            double exponent = Math.Pow(10, Math.Floor(Math.Log10(target))), normalized = target / exponent;
            double nice = normalized < 1.5 ? 1 : normalized < 3.5 ? 2 : normalized < 7.5 ? 5 : 10;
            return nice * exponent;
        }

        private string FormatRelativeTime(double timeMs)
        {
            double seconds = _result == null ? 0 : Math.Max(0, (timeMs - _result.FirstTimeMs) / 1000.0);
            TimeSpan t = TimeSpan.FromSeconds(seconds);
            return string.Format(CultureInfo.InvariantCulture, "{0:00}:{1:00}:{2:00}.{3:00}", (int)t.TotalHours, t.Minutes, t.Seconds, t.Milliseconds / 10);
        }

        private static void DrawCentered(Graphics g, string text, Rectangle rect)
        {
            SizeF size = g.MeasureString(text, SystemFonts.DefaultFont);
            using (var brush = new SolidBrush(UiPalette.Muted))
                g.DrawString(text, SystemFonts.DefaultFont, brush, rect.Left + (rect.Width - size.Width) / 2f, rect.Top + (rect.Height - size.Height) / 2f);
        }
    }

    internal sealed class GpsTimeComparer : IComparer<GpsSample>
    {
        public static readonly GpsTimeComparer Instance = new GpsTimeComparer();
        public int Compare(GpsSample x, GpsSample y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            return x.TimeMs.CompareTo(y.TimeMs);
        }
    }

    internal struct PointD
    {
        public double X;
        public double Y;
        public PointD(double x, double y) { X = x; Y = y; }
    }

    internal struct ViewSpans
    {
        public double X;
        public double Y;
    }

    internal sealed class CounterPeak
    {
        public double First;
        public double Last;
        public double Min;
        public double Max;
        public double TimeMs;
        public int Line;
        public string Phase;
        public bool Cumulative;
    }
}
