import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  createForumCategory,
  createForumComment,
  createForumTopic,
  getForumCategories,
  getForumTopic,
  getForumTopics,
  markSolution,
} from "@/api/forum";

export const useForumCategories = () =>
  useQuery({
    queryKey: ["forum", "categories"],
    queryFn: getForumCategories,
  });

export const useCreateCategory = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: createForumCategory,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["forum", "categories"] });
    },
  });
};

export const useForumTopics = (categoryId?: number) =>
  useQuery({
    queryKey: ["forum", "topics", categoryId],
    queryFn: () => getForumTopics(categoryId),
  });

export const useForumTopic = (id: number) =>
  useQuery({
    queryKey: ["forum", "topic", id],
    queryFn: () => getForumTopic(id),
  });

export const useCreateTopic = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: createForumTopic,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["forum", "topics"] });
      queryClient.invalidateQueries({ queryKey: ["forum", "categories"] });
    },
  });
};

export const useCreateComment = (topicId: number) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (content: string) => createForumComment(topicId, content),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["forum", "topic", topicId] });
      queryClient.invalidateQueries({ queryKey: ["forum", "topics"] });
    },
  });
};

export const useMarkSolution = (topicId: number) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (commentId: number) => markSolution(commentId, topicId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["forum", "topic", topicId] });
    },
  });
};
