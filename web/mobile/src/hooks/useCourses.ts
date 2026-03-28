import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { getCategories, getCourse, getCourseLessons, getCourses, submitTask } from "../api/courses";

export const useCategories = () =>
  useQuery({
    queryKey: ["categories"],
    queryFn: getCategories,
  });

export const useCourses = () =>
  useQuery({
    queryKey: ["courses"],
    queryFn: getCourses,
  });

export const useCourse = (id: number) =>
  useQuery({
    queryKey: ["courses", id],
    queryFn: () => getCourse(id),
  });

export const useCourseLessons = (courseId: number) =>
  useQuery({
    queryKey: ["courses", courseId, "lessons"],
    queryFn: () => getCourseLessons(courseId),
  });

export const useSubmitTask = (courseId: number) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (params: {
      lessonId: number;
      taskId: number;
      body: { selectedOptionIds: string[] } | { solved: boolean };
    }) => submitTask(params.lessonId, params.taskId, params.body),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["courses", courseId, "lessons"] });
      queryClient.invalidateQueries({ queryKey: ["profile"] });
    },
  });
};
