using UnityEngine;
using TMPro;

namespace TLab.WebView.Widget.Sample
{
    public class AlertDialogSample : MonoBehaviour, IAlertDialogHandler, IWidgetHandler<AlertDialog.Init, AlertDialog>
    {
        [SerializeField] private GameObject m_content;
        [SerializeField] private TextMeshProUGUI m_positive;
        [SerializeField] private TextMeshProUGUI m_neutral;
        [SerializeField] private TextMeshProUGUI m_negative;
        [SerializeField] private TextMeshProUGUI m_title;
        [SerializeField] private TextMeshProUGUI m_message;
        [SerializeField] private BrowserInputListener m_inputListener;

        private AlertDialog m_dialog;

        private string THIS_NAME => "[" + this.GetType() + "] ";

        private void ModifyTextMeshPro(TextMeshProUGUI textMesh, bool active, string text)
        {
            textMesh.text = text;
            textMesh.transform.parent.gameObject.SetActive(active);
        }

        public static string POSITIVE = "Positive";
        public static string NEGATIVE = "Negative";
        public static string NEUTRAL = "Neutral";

        public void Close()
        {
            m_dialog = null;

            m_content.SetActive(false);

            m_inputListener.enabled = true;

            ModifyTextMeshPro(m_positive, false, POSITIVE);
            ModifyTextMeshPro(m_neutral, false, NEUTRAL);
            ModifyTextMeshPro(m_negative, false, NEGATIVE);
        }

        public void OnDialog(AlertDialog.Init init, AlertDialog dialog)
        {
            m_dialog = dialog;

            m_inputListener.enabled = false;

            ModifyTextMeshPro(m_positive, init.positive, init.positiveLabel);
            ModifyTextMeshPro(m_neutral, init.neutral, init.neutralLabel);
            ModifyTextMeshPro(m_negative, init.negative, init.negativeLabel);
            ModifyTextMeshPro(m_title, init.title != "", init.title);
            ModifyTextMeshPro(m_message, !m_dialog.hasOverlay && (init.message != ""), init.message);

            m_content.SetActive(true);
        }

        public void Positive() => m_dialog?.Post(AlertDialog.Result.POSITIVE);

        public void Neutral() => m_dialog?.Post(AlertDialog.Result.NEUTRAL);

        public void Negative() => m_dialog?.Post(AlertDialog.Result.NEGATIVE);
    }
}
