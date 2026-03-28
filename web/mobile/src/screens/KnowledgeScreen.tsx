import React from "react";
import { SafeAreaView, StyleSheet } from "react-native";
import { QuizChallenge } from "../components/QuizChallenge";
import { semantic } from "../theme/tokens";

export const KnowledgeScreen = () => (
  <SafeAreaView style={styles.root}>
    <QuizChallenge />
  </SafeAreaView>
);

const styles = StyleSheet.create({
  root: {
    flex: 1,
    backgroundColor: semantic.background,
  },
});
