using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class Emailer : MonoBehaviour
{
    public string surveyLink = "https://docs.google.com/forms/d/e/1FAIpQLSefi_b0LVGqkz6XwE_zkK9E4lpSfNlrBu9fGvMnrmr0lSpIvg/viewform?usp=sf_link";

    private string windowsPass = "mzvrbrvanmpvwnqi";
    private string macPass = "wtjyzufrhkddkecg";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SendEmail(string receiver)
    {
        var fromAddress = new MailAddress("hometown.service859@gmail.com", "HomeTown Bound Support");
        var toAddress = new MailAddress(receiver);
        // TODO: add passwords for mobile devices when the time comes
        string fromPassword = System.Environment.OSVersion.Platform == System.PlatformID.MacOSX ? macPass : windowsPass;
        string subject = "HomeTown Bound Personalization";
        string body = "Please use the following link to access a survey that will allow you to personalize your experience while playing HomeTown Bound! "
            + "We highly recommend you fill out the questionaire on your personal device which contains many pictures of your family members.\n\n"
            + surveyLink
            + "\n\nThank you so much for your participation!\n-HomeTown Bound Support";

        var smtp = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
        };

        using (var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = body
        })
        {
            try
            {
                smtp.Send(message);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
    }

    public void OpenSurvey()
    {
        Application.OpenURL(surveyLink);
        gameObject.SetActive(false);
    }
    
}
