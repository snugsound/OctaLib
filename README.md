# OctaLib
This repo has been created to gather my thoughts with respect to a prospective project: OctaLib, a simple librarian app for the Elektron Octratrack.

Why should such a thing exist? While Octatrack is a great product, it's lacking some core librarian functionality, and with the abandonment of OctaEdit, no software editor exists.

Some simple operations--such as swapping banks within a projects--can be done directly on the filesystem. But more advanced operations involve rewriting Octatrack's proprietary files in a way that is beyond the average user.

## Product Vision

I'm considering a phased approach, starting with the basics, and building up from there.

### Phase 1

Provide a basic view of a project, with the ability to swap banks *within* a project. At a minimum, this should include:

- A grid layout showing occupied banks and patterns
- Part numbers for each pattern
- Part names for each bank

### Phase 2

- Add more advanced operations, such as swapping parts and patterns *between* banks.

### Future

- Reordering project samples (this will require rewriting banks and markers)
- Creating new composite projects from existing banks (i.e. copy bank 1 from project A, bank 2 from project B, etc.)

## Reverse Engineering

I have no intent to fully reverse engineer the file formats. If somebody else has done this, please consider sharing your findings with the community. My goal is to simply reverse engineer enough to achieve my immediate goals. 

### Project Files

An Octatrack project is compromised of 52 files:

- Project (1): `project`
- Arranger files (8): `arr01..arr08`
- Bank files (16): `bank01..bank16`
- Markers (1): `markers`

Each file has two versions: `.work` and `.strd`. My (untested but likely) assumption is that the `.strd` file is the "stored" (saved) file, and the `.work` is the working memory. Reloading a project likely replaces the `.work` with `.strd`, and saving does the inverse.

I will be largely ignoring the arranger and markers file for now. My assumption is that swapping banks will break arrangement references, but seeing as I don't use arrangements, I don't care enough to dig into this right now.

### The Project

Let's get the easy one out of the way first: the project file. This contains metadata about a project, including sample slot definitions, and it's the only project file stored in plain text.

I'm going to skip over the project metadata for now as it's not of interest to me.

#### Sample Definitions

The sample definitions are clearly delineated:
```
############################
# Samples
############################

<FLEX sample definitions>

<STATIC sample definitions>

############################
```

Sample definition format:
```
[SAMPLE]
TYPE=
SLOT=
PATH=
TRIM_BARSx100=
TSMODE=
LOOPMODE=
GAIN=
TRIGQUANTIZATION=
[/SAMPLE]

```

I can infer a few obvious things here but I'm not going to create a comprehensive list of all possible values at this time.

```
TYPE: FLEX or STATIC
SLOT: Slot number (001-128, plus 129-136 for FLEX)
PATH: ../path/to/file
TRIM_BARSx100: sample length in bars x100 (i.e 400=4 bars)
TSMODE: Time stretch mode
LOOPMODE: Loop mode
GAIN: Gain (default appears to be 48)
TRIGQUANTIZATION: Trig quantization (default appears to be -1)
```

Both lists are variable length, with up to 128 entries. The FLEX list has 8 special entries at the end (129-136) which I assume are for the recording buffers.

### Bank Files

My initial observations:

- It appears to be a fixed length file; this should make it easier to work with
- Unspecified values are padded with `FF`

Structurally, I see:

- A file header
- 16 PTRN (pattern) definitions containing
    - 8 TRAC definitions (tracks)
    - 8 MTRA definitions (MIDI tracks)
- A PART header
- The part names in plain text at the very end of the file

I also see this repeating pattern, which I assume is either a delineator, or some kind of metadata.

```
AA AA AA AA AA AA AA AA 00 00 00 00 00 00 00 00 10 02
```

What I will need to concretely identify in order to provide a visual representation of a project in phase 1:

* Which patterns are in use (I believe I've found a byte that captures this)
* Which patterns are using which part: this will requiring some probing
* Part names (easy)

Refer to the following file for a list of useful addresses:
https://github.com/snugsound/OctaLib/blob/main/OctaLib/Constants.cs