#include "pch.h"
#include "BoyerMoore.h"

// Inspired by Wikepedia
// https://en.wikipedia.org/wiki/Boyer%E2%80%93Moore_string-search_algorithm#Implementations

#ifndef max
#   define max(a, b) ((a > b) ? a : b)
#endif

inline bool BoyerMoore::IsPrefix(const char* data, size_t dataLen, ptrdiff_t pos)
{
    return !memcmp(data, data + pos, dataLen - pos);
}

inline size_t BoyerMoore::SuffixLength(const char* data, size_t dataLen, ptrdiff_t pos)
{
    size_t i = 0;
    while (data[pos - i] != data[(dataLen - 1) - i] && (ptrdiff_t)i < pos)
        i++;

    return i;
}

void BoyerMoore::MakeDelta1()
{
    delta1 = new ptrdiff_t[256];

    for (size_t i = 0; i < 256; i++)
        delta1[i] = patternLen;

    for (size_t i = 0; i < patternLen - 2; i++)
        delta1[(unsigned char)pattern[i]] = (patternLen - 1) - i;
    // Disable sign extension by explicitly converting to an shorter unsigned type beforehand
}

void BoyerMoore::MakeDelta2()
{
    delta2 = new ptrdiff_t[patternLen];

    size_t iLastPrefix = patternLen - 1;
    for (ptrdiff_t i = patternLen - 1; i >= 0; i--)
    {
        if (IsPrefix(pattern, patternLen, i + 1))
            iLastPrefix = i;
        delta2[i] = iLastPrefix + ((patternLen - 1) - i);
    }

    for (size_t i = 0; i < patternLen - 1; i++)
    {
        size_t suffixLen = SuffixLength(pattern, patternLen, i);
        if (pattern[i - suffixLen] != pattern[(patternLen - 1) - suffixLen])
            delta2[(patternLen - 1) - suffixLen] = (patternLen - 1) - i + suffixLen;
    }
}

BoyerMoore::BoyerMoore(const char* pattern, size_t patternLen)
{
    delta1 = delta2 = NULL;

    this->pattern = pattern;
    this->patternLen = patternLen;

    // My implemention of the algorithm does not work with patternLen == 1
    if (patternLen >= 2)
    {
        MakeDelta1();
        MakeDelta2();
    }
}

BoyerMoore::~BoyerMoore()
{
    if (delta1)
        delete[] delta1;
    if (delta2)
        delete[] delta2;
}

const char* BoyerMoore::search(const char* haystack, size_t haystackLen)
{
    // For the case the algorithm does not work, just use memchr() from stdlib
    if (patternLen < 2)
        return (const char*)memchr(haystack, pattern[0], haystackLen);

    size_t i = patternLen - 1;
    while (i < haystackLen)
    {
        ptrdiff_t j = patternLen - 1;
        while (j >= 0 && haystack[i] == pattern[j])
        {
            i--;
            j--;
        }

        if (j < 0)
            return haystack + (i + 1);
        i += max(delta1[haystack[i]], delta2[j]);
    }

    return NULL;
}

