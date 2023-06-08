// This file can be replaced during build by using the `fileReplacements` array.
// `ng build` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

const baseUrl = 'https://localhost:7254';
export const environment = {
  apiEndpoint: baseUrl,
  production: false,
  apiURL: baseUrl,
  profileThumbnailPicturePath: baseUrl + '/api/User/GetThumbnailProfilePictureByName/',
  profilePicturePath: baseUrl + '/api/User/GetProfilePictureByName/',
  picturePath: baseUrl + '/api/Common/GetImageById/',
  //recaptchaSiteKey: '6LeV7rgbAAAAALzFvoOVZCjI92o20Uk7PKkYqnRb',
  supportMail: 'amirhossein.gholmaitousi@gmail.com',
  whatsApp: '+98 938 206 4575',
  siteTitle: 'Ketabkhone',
  siteDescription: 'best online book store ever',
  siteKeywords: 'book-onlinebook-electronicbook-bookpdf-readonline'
};
