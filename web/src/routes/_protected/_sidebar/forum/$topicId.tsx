import { createFileRoute, Link, useParams } from "@tanstack/react-router";
import {
  ArrowLeftIcon,
  CheckCircle2Icon,
  EyeIcon,
  MessageSquareIcon,
  SendIcon,
} from "lucide-react";
import { useState } from "react";
import { useCreateComment, useForumTopic, useMarkSolution } from "@/hooks/useForum";
import { useMe } from "@/hooks/useMe";
import styles from "./$topicId.module.css";

const formatDate = (iso: string) => {
  const d = new Date(iso);
  return d.toLocaleDateString("ru-RU", {
    day: "numeric",
    month: "short",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
};

const ROLE_LABELS: Record<string, string> = {
  admin: "Администратор",
  teacher: "Преподаватель",
  student: "Ученик",
  parent: "Родитель",
};

const RouteComponent = () => {
  const { topicId } = useParams({ from: "/_protected/_sidebar/forum/$topicId" });
  const id = Number(topicId);
  const { data, isLoading, error } = useForumTopic(id);
  const { data: me } = useMe();
  const createComment = useCreateComment(id);
  const markSolutionMut = useMarkSolution(id);
  const [replyText, setReplyText] = useState("");

  if (isLoading) {
    return (
      <main className={styles.root}>
        <p>Загрузка...</p>
      </main>
    );
  }

  if (error || !data) {
    return (
      <main className={styles.root}>
        <p>Топик не найден</p>
      </main>
    );
  }

  const { topic, comments } = data;
  const isAuthor = me?.id === topic.author.id;

  const handleSubmitReply = (e: React.FormEvent) => {
    e.preventDefault();
    const trimmed = replyText.trim();
    if (!trimmed) return;

    createComment.mutate(trimmed, {
      onSuccess: () => setReplyText(""),
    });
  };

  const handleMarkSolution = (commentId: number) => {
    markSolutionMut.mutate(commentId);
  };

  return (
    <main className={styles.root}>
      <Link to="/forum" className={styles.backLink}>
        <ArrowLeftIcon size={16} />
        Назад к форуму
      </Link>

      <div className={styles.topicCard}>
        <div className={styles.topicTitleRow}>
          <h1 className={styles.topicTitle}>{topic.title}</h1>
          {topic.isSolved && (
            <span className={styles.solvedBadge}>
              <CheckCircle2Icon size={14} />
              Решено
            </span>
          )}
        </div>

        <p className={styles.topicContent}>{topic.content}</p>

        <div className={styles.topicFooter}>
          <div className={styles.authorRow}>
            {topic.author.avatar ? (
              <img src={topic.author.avatar} alt="" className={styles.avatarMd} />
            ) : (
              <div className={styles.avatarMd} />
            )}
            <div className={styles.authorInfo}>
              <span className={styles.authorName}>{topic.author.fullName}</span>
              <span className={styles.authorRole}>
                {ROLE_LABELS[topic.author.role] ?? topic.author.role}
              </span>
            </div>
          </div>
          <div className={styles.topicMeta}>
            <span className={styles.metaItem}>
              <EyeIcon />
              {topic.views}
            </span>
            <span className={styles.metaItem}>
              <MessageSquareIcon />
              {topic.replies}
            </span>
            <span>{formatDate(topic.createdAt)}</span>
          </div>
        </div>
      </div>

      <div className={styles.commentsSection}>
        <h2 className={styles.commentsTitle}>Ответы ({comments?.length ?? 0})</h2>

        {comments && comments.length > 0 ? (
          comments.map((comment) => (
            <div
              key={comment.id}
              className={`${styles.commentCard} ${comment.isSolution ? styles.solutionCard : ""}`}
            >
              {comment.isSolution && (
                <span className={styles.solutionLabel}>
                  <CheckCircle2Icon size={14} />
                  Решение
                </span>
              )}
              <p className={styles.commentContent}>{comment.content}</p>
              <div className={styles.commentFooter}>
                <div className={styles.authorRow}>
                  {comment.author.avatar ? (
                    <img src={comment.author.avatar} alt="" className={styles.avatarMd} />
                  ) : (
                    <div className={styles.avatarMd} />
                  )}
                  <div className={styles.authorInfo}>
                    <span className={styles.authorName}>{comment.author.fullName}</span>
                    <span className={styles.authorRole}>
                      {ROLE_LABELS[comment.author.role] ?? comment.author.role}
                    </span>
                  </div>
                </div>
                <span className={styles.commentDate}>{formatDate(comment.createdAt)}</span>
                {isAuthor && !topic.isSolved && !comment.isSolution && (
                  <button
                    type="button"
                    className={styles.markSolutionBtn}
                    onClick={() => handleMarkSolution(comment.id)}
                    disabled={markSolutionMut.isPending}
                  >
                    <CheckCircle2Icon size={14} />
                    Отметить решением
                  </button>
                )}
              </div>
            </div>
          ))
        ) : (
          <p className={styles.emptyComments}>Пока нет ответов. Будьте первым!</p>
        )}
      </div>

      <form className={styles.replyForm} onSubmit={handleSubmitReply}>
        <span className={styles.replyLabel}>Ваш ответ</span>
        <textarea
          className={styles.replyTextarea}
          placeholder="Напишите ваш ответ..."
          value={replyText}
          onChange={(e) => setReplyText(e.target.value)}
        />
        <div className={styles.replyActions}>
          <button
            type="submit"
            className={styles.submitBtn}
            disabled={!replyText.trim() || createComment.isPending}
          >
            <SendIcon size={16} />
            Отправить
          </button>
        </div>
      </form>
    </main>
  );
};

export const Route = createFileRoute("/_protected/_sidebar/forum/$topicId")({
  component: RouteComponent,
});
