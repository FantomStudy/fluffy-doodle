import React from "react";
import { SafeAreaView, StyleSheet } from "react-native";
import { FlowchartChallenge } from "../components/FlowchartChallenge";
import { semantic } from "../theme/tokens";

export const AlgorithmScreen = () => (
  <SafeAreaView style={styles.root}>
    <FlowchartChallenge />
  </SafeAreaView>
);

const styles = StyleSheet.create({
  root: {
    flex: 1,
    backgroundColor: semantic.background,
  },
});
