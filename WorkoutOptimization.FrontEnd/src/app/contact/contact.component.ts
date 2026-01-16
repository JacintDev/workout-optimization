import { Component } from '@angular/core';
import { EmailRequest } from '../../models/EmailRequest';
import emailjs from '@emailjs/browser';

@Component({
  selector: 'app-contact',
  standalone: false,
  templateUrl: './contact.component.html',
  styleUrl: './contact.component.sass',
})
export class ContactComponent {
  emailRequestModel: EmailRequest = {
    name: '',
    subject: '',
    email: '',
    message: '',
  };

  public async SendEmail() {
    emailjs.init('UXNv5Ku3Ltsnu1LtH');
    try {
      const response = await emailjs.send(
        'service_081v15b',
        'template_ot4rw4y',
        this.emailRequestModel as any,
        'UXNv5Ku3Ltsnu1LtH'
      );
      console.log(
        'Email has been sent successfully:',
        response.status,
        response.text
      );
      alert('Email has been sent successfully!');
      this.emailRequestModel = {
        name: '',
        subject: '',
        email: '',
        message: '',
      };
    } catch (error) {
      console.error('There was an error sending the email:', error);
      alert('There was an error sending the email. Please try again later.');
    }
  }
}
