class LeaderBoard {
	static inProgress = false;
	static createLeaderBoard(scoresJson, includeScore, userName) {
		this.inProgress = true;
		const wrapper = HtmlUtils.createElement('div', 'leaderBoardWrapper');
		const increasePairs = [];
		let counter = 1;
		for (const player of scoresJson) {
			const entryWrapper = HtmlUtils.addElement('div', 'leaderBoardEntry', wrapper);
			const name = HtmlUtils.addElement('span', '', entryWrapper);
			name.innerText = counter + ': ' + player.name;
			if (userName && player.name.toLowerCase() === userName.toLowerCase()) {
				entryWrapper.classList.add('currentUser');
			}

			const scoreWrapper = HtmlUtils.addElement('div', 'leaderBoardScoreWrapper', entryWrapper);
			const score = HtmlUtils.addElement('span', '', scoreWrapper);

			if (player.increase > 0 && includeScore) {
				const increase = HtmlUtils.addElement('span', 'leaderBoardIncrease', scoreWrapper);
				increase.innerText = player.increase;
				increasePairs.push([score, increase]);
				score.innerText = player.score - player.increase;
			} else {
				score.innerText = player.score;
			}

			counter++;
		}

		if (increasePairs.length > 0) {
			setTimeout(() => this.increasePair(increasePairs), 1000);
		} else {
			this.inProgress = false;
		}

		return wrapper;
	}

	static increasePair(pairs) {
		for (const pair of pairs) {
			const currentScore = parseInt(pair[0].innerText);
			const increaseLeft = parseInt(pair[1].innerText);
			const decrease = increaseLeft >= 15 ? 15 : increaseLeft;
			pair[1].innerText = increaseLeft - decrease;
			pair[0].innerText = currentScore + decrease;
			if (increaseLeft - decrease < 1) {
				pair[1].remove();
				pair[1] = null;
			}
		}

		pairs = pairs.filter(x => x[1] !== null);
		if (pairs.length > 0) {
			setTimeout(() => this.increasePair(pairs), 100);
		} else {
			this.inProgress = false;
		}
	}
}
