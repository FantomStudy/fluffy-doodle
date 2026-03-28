import React from "react";
import { SafeAreaView, StyleSheet } from "react-native";
import { CodingChallenge } from "../components/CodingChallenge";
import { semantic } from "../theme/tokens";

export const ChallengeScreen = () => (
  <SafeAreaView style={styles.root}>
    <CodingChallenge />
  </SafeAreaView>
);

const styles = StyleSheet.create({
  root: {
    flex: 1,
    backgroundColor: semantic.background,
  },
});
