#pragma once

// The Boyer-Moore binary searcher
class BoyerMoore final
{
private:

    const char* pattern;
    size_t patternLen;

    ptrdiff_t* delta1;
    ptrdiff_t* delta2;

    // Copying an instance is not allowed
    BoyerMoore(const BoyerMoore& x) = delete;
    BoyerMoore& operator=(BoyerMoore& x) = delete;

    static inline bool IsPrefix(const char* data, size_t dataLen, ptrdiff_t pos);
    static inline size_t SuffixLength(const char* data, size_t dataLen, ptrdiff_t pos);

    void MakeDelta1();
    void MakeDelta2();


public:

    // Construct a search context using the given pattern
    // NOTE: the pattern is NOT saved inside the object and must stay available during the searching process
    BoyerMoore(const char* pattern, size_t patternLen);
    ~BoyerMoore();

    // Search for the first occurance of pattern in haystack
    // Returns the location in haystack of found, or NULL otherwise
    const char* search(const char* haystack, size_t haystackLen);
};

