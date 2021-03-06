﻿using com.epicface.vodkabets.networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.epicface.vodkabets.crash {

	public class CrashBetHandler : MonoBehaviour {

		[SerializeField] int minBet;
		int bet;
		static bool allowWithdraw;
		int betEntryIndex;

		GameObject player;
		InputField betField;
		static GameObject disabledPanel;
		static GameObject withdrawPanel;
		static Text withdrawButtonText;

		// Use this for initialization
		void Start () {
			betField = transform.FindChild("BetField").GetComponent<InputField>();
			disabledPanel = transform.FindChild("DisabledOverlay").gameObject;
			withdrawPanel = transform.FindChild("Withdraw Panel").gameObject;
			withdrawButtonText = withdrawPanel.transform.FindChild("Withdraw Bet Button/Text").GetComponent<Text>();
		}

		// Update is called every frame
		void Update () {
			if (player == null) {
				player = GameObject.FindGameObjectWithTag("LocalPlayer");
				return;
			} else if (withdrawPanel.activeInHierarchy) {
				//do stuff for withdraw
				withdrawButtonText.text = "Cashout @ " + string.Format("{0:0.00}", CrashGrapher.multiplier) + "x";
				return;
			}

			bet = int.Parse(betField.text);
		}
		
		public void DoubleBet () {
			if (player == null) {
				return;
			}

			betField.text = (2*bet).ToString();
		}

		public void HalfBet () {
			if (player == null) {
				return;
			}

			betField.text = Mathf.RoundToInt(bet/2).ToString();
		}

		public void MaxBet () {
			if (player == null) {
				return;
			}

			betField.text = player.GetComponent<Client>().Vlads.ToString();
		}

		public void MinBet () {
			if (player == null) {
				return;
			}
			
			betField.text = minBet.ToString();
		}

		public void PlaceBet () {
			if (player == null) {
				return;
			}

			//make sure bet is valid
			Client playerInfo = player.GetComponent<Client>();
			if (bet <= playerInfo.Vlads) {
				disabledPanel.SetActive(true);

				//handle bet
				betEntryIndex = CrashBetEntryUtility.CreateEntry(playerInfo.Username, bet);
				allowWithdraw = true;

				playerInfo.Vlads -= bet;
			}
		}

		public void WithdrawBet () {
			CrashBetEntryUtility.WithdrawEntry(betEntryIndex, bet, CrashGrapher.multiplier);
			
			//change add profit to vlads
			Client playerInfo = player.GetComponent<Client>();
			playerInfo.Vlads += Mathf.RoundToInt(bet * CrashGrapher.multiplier);
		}

		public static void DisableBetting () {
			disabledPanel.SetActive(true);
		}

		public static void EnableWithdraw () {
			if (!allowWithdraw) {
				return;
			}
			allowWithdraw = false; //disable withdraw for next round

			disabledPanel.SetActive(false);
			withdrawPanel.SetActive(true);

		}

		public static void ResetBetPanel () {
			disabledPanel.SetActive(false);
			withdrawPanel.SetActive(false);
		}

		public void RoundBet () {
			//clamps bet between minBet and the player's Vlads count
			betField.text = Mathf.Clamp(bet, minBet, player.GetComponent<Client>().Vlads).ToString();
		}
	}

}
