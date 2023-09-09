class Alert {
	static openAlert(message) {
		const okButton = HtmlUtils.createElement('button', 'sectionButton');
		okButton.innerText = 'Ok';
		$(okButton).click(() => Popup.closePopup());
		Alert.createAlertBox(message, [okButton]);
	}

	static openDialog(message, okAction) {
		const okButton = HtmlUtils.createElement('button', 'sectionButton');
		okButton.innerText = 'Ja';
		$(okButton).click(() => okAction());

		const cancelButton = HtmlUtils.createElement('button', 'sectionButton');
		cancelButton.innerText = 'Avbryt';
		$(cancelButton).click(() => Popup.closePopup());
		Alert.createAlertBox(message, [okButton, cancelButton]);
	}

	static createAlertBox(message, buttons) {
		const alertBox = HtmlUtils.createElement('div', 'alertBox');
		const heading = HtmlUtils.addElement('span', 'alertMessage', alertBox);
		heading.innerText = message;
		const buttonWrapper = HtmlUtils.addElement('div', 'alertButtons', alertBox);
		for (const btn of buttons) {
			buttonWrapper.appendChild(btn);
		}

		Popup.putOnOverLay(alertBox, false);
	}
}
