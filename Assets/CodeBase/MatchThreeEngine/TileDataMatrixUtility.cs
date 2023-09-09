using System;
using System.Collections.Generic;


	public static class TileDataMatrixUtility
	{
		public static void Swap(int x1, int y1, int x2, int y2, TileData[,] tiles)
		{
			var tile1 = tiles[x1, y1];

			tiles[x1, y1] = tiles[x2, y2];

			tiles[x2, y2] = tile1;
		}

		public static (TileData[], TileData[]) GetConnections(int originX, int originY, TileData[,] tiles)
		{
			var origin = tiles[originX, originY];
			var width = tiles.GetLength(0);
			var height = tiles.GetLength(1);
			var horizontalConnections = new TileData[width];
			var verticalConnections = new TileData[height];
			var horizontalCount = 0;
			var verticalCount = 0;
			for (var x = originX - 1; x >= 0; x--)
			{
				var other = tiles[x, originY];
				if (other.TypeId != origin.TypeId) break;
				horizontalConnections[horizontalCount] = other;
				horizontalCount++;
			}
			for (var x = originX + 1; x < width; x++)
			{
				var other = tiles[x, originY];
				if (other.TypeId != origin.TypeId) break;
				horizontalConnections[horizontalCount] = other;
				horizontalCount++;
			}
			for (var y = originY - 1; y >= 0; y--)
			{
				var other = tiles[originX, y];
				if (other.TypeId != origin.TypeId) break;
				verticalConnections[verticalCount] = other;
				verticalCount++;
			}
			for (var y = originY + 1; y < height; y++)
			{
				var other = tiles[originX, y];
				if (other.TypeId != origin.TypeId) break;
				verticalConnections[verticalCount] = other;
				verticalCount++;
			}
			Array.Resize(ref horizontalConnections, horizontalCount);
			Array.Resize(ref verticalConnections, verticalCount);
			return (horizontalConnections, verticalConnections);
		}

		public static Match FindPossibleMatch(TileData[,] tiles)
		{
			var bestMatch = default(Match);

			for (var y = 0; y < tiles.GetLength(1); y++)
			{
				for (var x = 0; x < tiles.GetLength(0); x++)
				{
					var tile = tiles[x, y];

					var (h, v) = GetConnections(x, y, tiles);

					var match = new Match(tile, h, v);

					if (match.Score < 0) continue;

					if (bestMatch == null)
					{
						bestMatch = match;
					}
					else if (match.Score > bestMatch.Score) bestMatch = match;
				}
			}

			return bestMatch;
		}

		private static (int, int) GetDirectionOffset(byte direction) => direction switch
		{
			0 => (-1, 0),
			1 => (0, -1),
			2 => (1, 0),
			3 => (0, 1),

			_ => (0, 0),
		};

		public static Move FindPossibleMove(TileData[,] tiles)
		{
			var tilesCopy = (TileData[,])tiles.Clone();

			var width = tilesCopy.GetLength(0);
			var height = tilesCopy.GetLength(1);

			var bestScore = int.MinValue;

			var bestMove = default(Move);

			for (var y = 0; y < height; y++)
			{
				for (var x = 0; x < width; x++)
				{
					for (byte d = 0; d <= 3; d++)
					{
						var (offsetX, offsetY) = GetDirectionOffset(d);

						var x2 = x + offsetX;
						var y2 = y + offsetY;

						if (x2 < 0 || x2 > width - 1 || y2 < 0 || y2 > height - 1) continue;

						Swap(x, y, x2, y2, tilesCopy);

						var match = FindPossibleMatch(tilesCopy);

						if (match != null && match.Score > bestScore)
						{
							bestMove = new Move(x, y, x2, y2);

							bestScore = match.Score;
						}

						Swap(x, y, x2, y2, tilesCopy);
					}
				}
			}

			return bestMove;
		}
	}

