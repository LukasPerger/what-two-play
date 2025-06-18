import {z} from 'zod';

// Friend related schemas
export const friendResponseSchema = z.object({
  steamid: z.string(),
  relationship: z.string(),
  friend_since: z.number(),
  persona_name: z.string(),
  avatar_full: z.string(),
});

export const friendsListSchema = z.object({
  friends: z.array(friendResponseSchema),
});

export const friendListResponseSchema = z.object({
  friendslist: friendsListSchema,
});

// App details related schemas
export const categorySchema = z.object({
  id: z.number(),
  description: z.string(),
});

export const genreSchema = z.object({
  id: z.string(),
  description: z.string(),
});

export const appDetailsSchema = z.object({
  categories: z.array(categorySchema),
  genres: z.array(genreSchema),
});

export const appDetailsResponseSchema = z.object({
  data: appDetailsSchema,
});

// Owned games related schemas
export const ownedGameSchema = z.object({
  appid: z.number(),
  name: z.string(),
  playtime_forever: z.number(),
  img_icon_url: z.string(),
  has_community_visible_stats: z.boolean(),
  playtime_windows_forever: z.number(),
  playtime_mac_forever: z.number(),
  playtime_linux_forever: z.number(),
  tags: z.array(z.string())
});

export const ownedGamesSchema = z.object({
  game_count: z.number(),
  games: z.array(ownedGameSchema),
});

export const ownedGamesResponseSchema = z.object({
  response: ownedGamesSchema,
});

// Player summaries related schemas
export const playerSummarySchema = z.object({
  steamid: z.string(),
  personaname: z.string(),
  avatarfull: z.string(),
});

export const playerSummariesListSchema = z.object({
  players: z.array(playerSummarySchema),
});

export const playerSummariesResponseSchema = z.object({
  response: playerSummariesListSchema,
});

// Type exports
export type FriendResponse = z.infer<typeof friendResponseSchema>;
export type FriendsList = z.infer<typeof friendsListSchema>;
export type FriendListResponse = z.infer<typeof friendListResponseSchema>;

export type Category = z.infer<typeof categorySchema>;
export type Genre = z.infer<typeof genreSchema>;
export type AppDetails = z.infer<typeof appDetailsSchema>;
export type AppDetailsResponse = z.infer<typeof appDetailsResponseSchema>;

export type OwnedGame = z.infer<typeof ownedGameSchema>;
export type OwnedGames = z.infer<typeof ownedGamesSchema>;
export type OwnedGamesResponse = z.infer<typeof ownedGamesResponseSchema>;

export type PlayerSummary = z.infer<typeof playerSummarySchema>;
export type PlayerSummariesList = z.infer<typeof playerSummariesListSchema>;
export type PlayerSummariesResponse = z.infer<typeof playerSummariesResponseSchema>;
