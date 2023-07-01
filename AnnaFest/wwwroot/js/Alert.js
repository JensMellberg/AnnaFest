class Alert {
	static openAlert(message) {
		const alertBox = HtmlUtils.createElement('div', 'alertBox');
		const heading = HtmlUtils.addElement('span', 'alertMessage', alertBox);
		heading.innerText = message;
		const buttonWrapper = HtmlUtils.addElement('div', 'alertButtons', alertBox);
		const okButton = HtmlUtils.addElement('button', 'sectionButton', buttonWrapper);
		okButton.innerText = 'Ok';
		$(okButton).click(() => Popup.closePopup());

		Popup.putOnOverLay(alertBox, false);
	}
}
