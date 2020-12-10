#version 330

#ifndef ARB_COMMON_GLSL_INCLUDED
#define ARB_COMMON_GLSL_INCLUDED

#define GetCC(a) ((a) > 0 ? 1 : (a) < 0 ? -1 : 0)
#define BCCEQ(a) ((a) == 0)
#define BCCGE(a) ((a) >= 0)
#define BCCGT(a) ((a) > 0)
#define BCCLE(a) ((a) <= 0)
#define BCCLT(a) ((a) < 0)
#define BCCNE(a) ((a) != 0)
#define BCCTR(a) (true)
#define BCCFL(a) (false)

#define CCTR(a, b, c) (true ? b : c)
#define CCFL(a, b, c) (false ? b : c)
#define CCEQ(a, b, c) ((a) == 0 ? b : c)
#define CCGE(a, b, c) ((a) >= 0 ? b : c)
#define CCGT(a, b, c) ((a) > 0 ? b : c)
#define CCLE(a, b, c) ((a) <= 0 ? b : c)
#define CCLT(a, b, c) ((a) < 0 ? b : c)
#define CCNE(a, b, c) ((a) != 0 ? b : c)
#define CCTR(a, b, c) (true ? b : c)
#define CCFL(a, b, c) (false ? b : c)

#define GetCCVec(a, b) mix(mix(vec##b(0), vec##b(-1), lessThan(a, vec##b(0))), vec##b(1), greaterThan(a, vec##b(0)))
#define CCEQVec(a, b, c, d) mix(c, d, equal(a, vec##b(0)))
#define CCGEVec(a, b, c, d) mix(c, d, greaterThanEqual(a, vec##b(0)))
#define CCGTVec(a, b, c, d) mix(c, d, greaterThan(a, vec##b(0)))
#define CCLEVec(a, b, c, d) mix(c, d, lessThanEqual(a, vec##b(0)))
#define CCLTVec(a, b, c, d) mix(c, d, lessThan(a, vec##b(0)))
#define CCNEVec(a, b, c, d) mix(c, d, notEqual(a, vec##b(0)))
#define CCTRVec(a, b, c, d) mix(c, d, bvec##b(true))
#define CCFLVec(a, b, c, d) mix(c, d, bvec##b(false))

#endif